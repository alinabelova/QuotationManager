import axios from 'axios';


export default {
    name: 'app-account',
    components: {
    },
    data: function () {
        return {
            languages: [{ text: 'English', value: 'en' }, { text: 'Russian', value: 'ru' }, { text: 'Armenian', value: 'hy' }],
            dialog: false,
            email: '',
            emailRules: [
                (v) => !!v || 'Поле \"E-mail\" обязательно',
                (v) => /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(v) || 'Значение поля \"E-mail\" некорректно'
            ],
            valid: true,
            password: '',
            passwordRules: [
                (v) => !!v || 'Поле \"Пароль\" обязательно',
                (v) => v && v.length > 5 || 'Поле \"Пароль\" должно содержать более 6 символов',
                (v) => /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{6,})/.test(v) || '\"Пароль\" должен содержать более 1й цифры, более 1й буквы в верхнем и нижнем регистрах'
            ],
            confirmPassword: '',
            __RequestVerificationToken: '',
            dialogMessage: false,
            dialogMessageText: '',
            dialogMessageTitle: '',
            errors: []
        }
    },
    computed: {
        isError: function () {
            return this.errors.length > 0;
        }
    },
    watch: {
        
    },
    mounted: function () {
        var tokenElements = document.getElementsByName("__RequestVerificationToken");
        if (tokenElements.length) {
            this.__RequestVerificationToken = tokenElements[0].value;
        }
    },
    methods: {
        showMessage: function(title, text) {
            this.dialogMessageTitle = title;
            this.dialogMessageText = text;
            this.dialogMessage = true;
        },
        errorMessage: function(data) {
            var errorValue = data.value;
            this.showMessage('Ошибка', errorValue);
        },
        login: function () {
            var componentData = this;
            var formData = new FormData();
            formData.append('Email', this.email);
            formData.append('Password', this.password);
            formData.append('__RequestVerificationToken', this.__RequestVerificationToken);
            axios({
                    method: 'post',
                    url: '/Account/Login',
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    },
                    params: {
                        'returnurl': '/'
                    },
                    data: formData
                })

                .then(function (response) {
                    window.location = response.data;
                })
                .catch(function (error) {
                    console.log(error);
                    componentData.showMessage('Ошибка', error.response.data);
                });
        },
        close() {
            this.dialog = false;
        },

        save() {
            this.$validator.validateAll().then((result) => {
                if (result) {
                    if (this.password !== this.confirmPassword) {
                        this.showMessage('Ошибка', 'Введеные пароли не совпадают!');
                        return;
                    } else {
                        var componentData = this;
                        var formData = new FormData();
                        formData.append('Email', this.email);
                        formData.append('Password', this.password);
                        formData.append('ConfirmPassword', this.confirmPassword);
                        formData.append('__RequestVerificationToken', this.__RequestVerificationToken);
                        axios({
                                method: 'post',
                                url: '/Account/Register',
                                headers: {
                                    'Content-Type': 'multipart/form-data'
                                },
                                params: {
                                    'returnurl': '/'
                                },
                                data: formData
                            })
                            .then(function(response) {
                                window.location = response.data;
                            })
                            .catch(function(error) {
                                console.log(error);
                                componentData.showMessage('Ошибка', error.response.data);
                                console.log(error);
                            });
                    }
                    this.close();
                    return;
                }
                alert('Ошибки валидации!');
            });
        }
    }
}