import axios from 'axios';
import moment from 'moment';
import DatePicker from '../DatePicker/DatePicker.vue.html';

export default {
    components: {
        DatePicker
    },
    data: () => ({
        dialog: false,
        headers: [
            { text: 'Город', align: 'left', value: 'city' },
            { text: 'Сумма', value: 'refinancingAmount' },
            { text: 'Цель рефинансирования', sortable: false, value: 'refinancingTarget' },
            { text: '%', value: 'interestRate' },
            { text: 'Комментарий', sortable: false, value: 'comment' },
            { text: 'Дата создания', value: 'createdAt' },
            { text: 'Дата редактирования', sortable: false, value: 'modifiedAt' }
        ],
        search: '',
        totalQuotas: 0,
        loading: true,
        pagination: {},
        quotas: [],
        targets: [],
        cities: [],
        contributions: [],
        editedIndex: -1,
        editedItem: {
            interestRate: 0,
            additionalContributions: []
        },
        defaultItem: {},
        valid: true
    }),

    computed: {
        formTitle() {
            return this.editedIndex === -1 ? 'Создать квоту' : 'Редактировать';
        }
    },

    watch: {
        pagination: {
            handler() {
                this.initialize();
            },
            deep: true
        },
        dialog(val) {
            val || this.close();
        }
    },
    filters: {
        russianDate: function (date) {
            if (date)
                return moment(date).format('DD.MM.YYYY HH:mm');
            else
                return '';
        },
        getCityName: function (cityId, cities) {
            if (cityId) {
                for (var c in cities) {
                    if (cities.hasOwnProperty(c)) {
                        if (cities[c].id === cityId) {
                            return cities[c].name;
                        }
                    }
                }
            }
            return '';
        },
        getEnumDisplayName: function (enumValue, targets) {
            for (var t in targets) {
                if (targets.hasOwnProperty(t)) {
                    if (t === enumValue.toString()) {
                        return targets[t].text;
                    }
                }
            }
            return '';
        }
    },
    created () {
        this.getLookups();
    },

    methods: {   
        initialize() {
            this.loading = true;
            var componentData = this;
            const { sortBy, descending, page, rowsPerPage } = this.pagination;
            axios.all([
                axios.post('/api/Quota/Get', { Pagination: this.pagination}),
                    axios.post('/api/Quota/GetTotalElements', {})
                ])
                .then(axios.spread(function (quotas, total) {
                    if (typeof (quotas.data) !== "object") {
                        return;
                    }
                    componentData.quotas = quotas.data;
                    componentData.totalQuotas = total.data;
                }))
                .catch(function(error) {
                        console.log(error);
                    }
                );
            this.loading = false;
        },

        getLookups() {
            var componentData = this;
            axios.post('/api/Quota/GetLookups', {})
                .then(function (lookups) {
                    if (typeof (lookups.data) !== "object") return;
                    componentData.cities = lookups.data.cities;
                    componentData.contributions = lookups.data.contributions;
                    var refTargets = lookups.data.refinancingTargets;
                    for (var t in refTargets) {
                        if (refTargets.hasOwnProperty(t)) {
                            componentData.targets.push({
                                text: t,
                                value: refTargets[t]
                            });
                        }
                    }
                })
                .catch(function(error) {
                    console.log(error);
                });
        },

        editItem (item) {
            this.editedIndex = this.quotas.indexOf(item);
            this.editedItem = Object.assign({}, item); 
            if (this.editedItem.createdAt) {
                this.editedItem.createdAtTime = moment(this.editedItem.createdAt).format('HH:mm');
            }
            if (this.editedItem.modifiedAt) {
                this.editedItem.modifiedAtTime = moment(this.editedItem.modifiedAt).format('HH:mm');
            }
            this.dialog = true;
        },

        deleteItem(item) {
            var componentData = this;
            const index = this.quotas.indexOf(item);
            confirm('Вы действительно хотите удалить элемент?') &&
                axios.post('/api/Quota/Remove?id=' + item.id)
                .then(function (response) {
                    if (typeof (response.data) !== "object") {
                        componentData.quotas.splice(index, 1);
                    }
                })
                .catch(function (error) {
                    console.log(error);
                });
        },

        downloadReport(item) {
            var city = this.cities.find(function (city) { return city.id === item.cityId },
                this);
            var target = this.targets.find(function (t) { return t.value === item.refinancingTarget },
                this);
            var report = "<html>" +
                "<head>" +
                "<meta charset=\"utf-8\">" +
                "<title>Отчет</title>" +
                "</head>" +
                "<body>" +
                "<br>" +
                "<p style = \"text-align: center;\"> <strong>Отчет по квоте № " +
                item.id +
                "</strong></p>" +
                "<div style=\"padding-left:20px;\">" +
                "<p>Сумма рефинанисрования: <em>" +
                item.refinancingAmount +
                "</em></p>" +
                "<p>Дата создания квоты: <em>" +
                moment(item.createdAt).format('DD.MM.YYYY HH:mm') +
                "</em></p>" +
                "<p>Дата генерации отчета: <em>" +
                moment().format('DD.MM.YYYY HH:mm') +
                "</em></p>" +
                "<p>Город: <em>" +
                city.name +
                "</em></p>" +
                "<p>Цель рефинансирования: <em>" +
                target.text +
                "</em></p>" +
                "<p>Итоговая процентная ставка: <em>" +
                item.interestRate +
                "</em></p>" +
                "<br>" +
                "<p><span style=\"text-decoration: underline;\">Дополнительные взносы</span></p>" +
                "<div>" +
                "<ul>";
            var addContributions = '';
            for (var ac in item.additionalContributions) {
                if (item.additionalContributions.hasOwnProperty(ac)) {
                    addContributions += "<li>" + item.additionalContributions[ac].amount + "</li>";
                }
            }
            report += addContributions + "</ul></body></html>";
            var htmlContent = [report];
            var bl = new Blob(htmlContent, { type: "text/html" });
            var a = document.createElement("a");
            a.href = URL.createObjectURL(bl);
            a.download = "Report #" + item.id + ".html";
            a.hidden = true;
            document.body.appendChild(a);
            a.click();
        },
        close() {
            this.dialog = false;
            setTimeout(() => {
                    this.editedItem = Object.assign({}, this.defaultItem);
                    this.editedIndex = -1;
                },
                300);
        },
        changeAdditionalContribution() {
            if (this.editedItem.cityId && this.editedItem.refinancingAmount > 0) {
                if (this.editedItem.additionalContributions && this.editedItem.additionalContributions.length > 0) {
                    this.editedItem.additionalContributions = [];
                }
                var city = this.cities.find(function (city) { return city.id === this.editedItem.cityId },
                    this);
                if (!city) return;
                var aditContributions = this.contributions;
                for (var ac in aditContributions) {
                    if (aditContributions.hasOwnProperty(ac)) {
                        if (aditContributions[ac].cityId === this.editedItem.cityId) {
                            if (!this.editedItem.additionalContributions) {
                                this.editedItem.additionalContributions = [];
                            } 
                            this.editedItem.additionalContributions.push({
                                contributionId: aditContributions[ac].id,
                                amount: (this.editedItem.refinancingAmount *
                                        city.significanceLevel *
                                        aditContributions[ac].baseAmount *
                                        0.0001)
                                    .toFixed(2)
                            });
                        }
                    }
                }
            }
        },
        changeRate() {
            if (this.editedItem.cityId && this.editedItem.refinancingTarget >= 0) {
                var level = '';
                for (var c in this.cities) {
                    if (this.cities.hasOwnProperty(c)) {
                        if (this.cities[c].id === this.editedItem.cityId) {
                            level = this.cities[c].significanceLevel;
                            break;
                        }
                    }
                }
                if (level === '')
                    return;
                this.editedItem.interestRate = level + 10;
                if (this.editedItem.refinancingTarget === 1) {
                    this.editedItem.interestRate *= 1.5;
                } else if (this.editedItem.refinancingTarget === 2) {
                    this.editedItem.interestRate *= 1.2;
                }
                this.editedItem.interestRate = this.editedItem.interestRate.toFixed(2);
            }
        },
        save() {
            this.$validator.validateAll().then((result) => {
                var componentData = this;
                if (result) {
                    if (this.editedItem.createdAtTime) {
                        this.editedItem.createdAt = moment(this.editedItem.createdAt).format('DD.MM.YYYY') + ' ' + this.editedItem.createdAtTime;
                    }
                    if (this.editedItem.modifiedAt && this.editedItem.modifiedAtTime) {
                        this.editedItem.modifiedAt = moment(this.editedItem.modifiedAt).format('DD.MM.YYYY') + ' ' + this.editedItem.modifiedAtTime;
                    }
                    if (this.editedIndex > -1) {
                        axios.post('/api/Quota/Update', { Quota: this.editedItem })
                            .then(function (response) {
                                if (typeof (response.data) !== "object") {
                                    componentData.quotas.push(componentData.editedItem);
                                    componentData.quotas[componentData.editedIndex].additionalContributions =
                                        componentData.editedItem.additionalContributions;
                                    Object.assign(componentData.quotas[componentData.editedIndex], componentData.editedItem);
                                }
                                componentData.close();
                            })
                            .catch(function (error) {
                                console.log(error);
                                componentData.close();
                            });
                    } else {
                        axios.post('/api/Quota/Create', { Quota: this.editedItem })
                            .then(function (response) {
                                if (typeof (response.data) !== "object") {
                                    debugger;
                                    componentData.editedItem.id = response.data;
                                    componentData.quotas.push(componentData.editedItem);
                                    componentData.quotas[componentData.editedIndex].additionalContributions =
                                        componentData.editedItem.additionalContributions;
                                }
                                componentData.close();
                            })
                            .catch(function (error) {
                                console.log(error);
                                componentData.close();
                            });
                    }
                    return;
                }
                alert('Ошибки валидации!');
            });

        }
    }
}