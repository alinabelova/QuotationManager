import axios from 'axios';
import moment from 'moment';

export default {
    data: () => ({
        dialog: false,
        headers: [
            {
                text: 'Город',
                align: 'left',
                sortable: false,
                value: 'city.name'
            },
            { text: 'Сумма рефинансирования', value: 'refinancingAmount' },
            { text: 'Цель рефинансирования', value: 'refinancingTarget' },
            { text: 'Комментарий', value: 'comment' },
            { text: 'Дата создания', value: 'createdAt' },
            { text: 'Дата редактирования', value: 'modifiedAt' }
        //{ text: 'Fat (g)', value: 'fat' },
            //{ text: 'Carbs (g)', value: 'carbs' },
            //{ text: 'Protein (g)', value: 'protein' },
            //{ text: 'Actions', value: 'name', sortable: false }
        ],
        quotas: [],
        targets: [],
        cities: [],
        contributions: [],
        editedIndex: -1,
        editedItem: {
            name: '',
            calories: 0,
            fat: 0,
            carbs: 0,
            protein: 0
        },
        defaultItem: {
            name: '',
            calories: 0,
            fat: 0,
            carbs: 0,
            protein: 0
        }
    }),

    computed: {
        formTitle() {
            return this.editedIndex === -1 ? 'Создать квоту' : 'Редактировать';
        }
    },

    watch: {
        dialog(val) {
            val || this.close();
        }
    },
    filters: {
        russianDate: function (date) {
            if (date)
                return moment(date).format('DD.MM.YYYY HH.mm');
            else
                return '';
        },
        getEnumDisplayName: function (enumValue, targets) {
            debugger;
            if (enumValue) {
                for (var t in targets) {
                    if (targets.hasOwnProperty(t)) {
                        if (t === enumValue.toString()) {
                            return targets[t].text;
                        }
                    }
                }
            }
            return '';
        }
    },
    created () {
        this.initialize();
    },

    methods: {
        initialize() {
            var componentData = this;
           // componentData.onloading(true);
            axios.all([
                    axios.post('/api/Quota/Get', {}),
                    axios.post('/api/Quota/GetLookups', {})
                ])
                .then(axios.spread(function(quotas, lookups) {
                    debugger;
                    if (typeof (quotas.data) !== "object" || typeof (lookups.data) !== "object") {
                        return;
                    }
                    componentData.quotas = quotas.data;
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
                }))
                .catch(function(error) {
                    debugger;
                    console.log(error);
                });
        },

        editItem (item) {
            this.editedIndex = this.quotas.indexOf(item);
            this.editedItem = Object.assign({}, item);
            this.dialog = true;
        },

        deleteItem (item) {
            const index = this.quotas.indexOf(item);
            confirm('Вы действительно хотите удалить элемент?') && this.quotas.splice(index, 1);
        },

        close () {
            this.dialog = false;
            setTimeout(() => {
                    this.editedItem = Object.assign({}, this.defaultItem);
                    this.editedIndex = -1;
                },
                300);
        },

        save () {
            if (this.editedIndex > -1) {
                Object.assign(this.quotas[this.editedIndex], this.editedItem);
            } else {
                this.quotas.push(this.editedItem);
            }
            this.close();
        }
    }
}