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
            dialogForgotPassword: false,
            dialogMessage: false,
            dialogMessageText: '',
            dialogMessageTitle: '',
            errors: []
        }
    },
    computed: {
        guestId: function () {
          return window.guestId;
        },
        isError: function () {
            return this.errors.length > 0;
        }
    },
    watch: {
        
    },
    mounted: function () {
        this.email = window.email;
        var lang = window.lang;
        if (lang) {
            lang = lang.toLowerCase().trim();
            if (["en", "ru", "hy"].indexOf(lang) !== -1) {
                this.language = lang;
            }
        }
        var tokenElements = document.getElementsByName("__RequestVerificationToken");
        if (tokenElements.length) {
            this.__RequestVerificationToken = tokenElements[0].value;
        }
    },
    methods: {
        label: function (fieldName) {
            var val = '';
            if (arguments.length > 1) {
                val = arguments[1];
            }
            
            return this.translate(fieldName, val);
        },
        showMessage: function(title, text) {
            this.dialogMessageTitle = title;
            this.dialogMessageText = text;
            this.dialogMessage = true;
        },
        errorMessage: function(data) {
            var errorMessage = data.message;
            var errorValue = data.value;
            this.showMessage('Ошибка', errorValue);
        },
        setPasswordLink: function() {
            this.dialogForgotPassword = true;
            this.$nextTick(function () {
                // кастыль появился с версией 0.15.7
                //this.$refs['opp'].focus();
                this.$refs['sendEmail'].$refs.input.focus();

            });
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
                    debugger;
                    window.location = response.data;
                })
                .catch(function (error) {
                    debugger;
                    console.log(error);
                    componentData.showMessage('Ошибка', error.response.data);
                    console.log(error);
                });
        },
        close() {
            debugger;
            this.dialog = false;
        },

        save() {
            debugger;
            if (!this.$refs.form.validate()) {
                return;
            }
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
                    .then(function (response) {
                        debugger;
                        window.location = response.data;
                    })
                    .catch(function (error) {
                        debugger;
                        console.log(error);
                        //componentData.errorMessage(error.response.data);
                        componentData.showMessage('Ошибка', error.response.data);
                        console.log(error);
                    });
            }
            this.close();
        },

        sendMail: function () {
            if (this.password !== this.confirmPassword) {
                alert('Введеные пароли не совпадают!');
            } else {
                var componentData = this;
                var formData = new FormData();
                formData.append('Email', this.email);
                formData.append('Password', this.language);
                formData.append('__RequestVerificationToken', this.__RequestVerificationToken);
                axios({
                        method: 'post',
                        url: '/Account/ForgotPassword',
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        },
                        data: formData
                    })
                    .then(function(response) {
                        componentData.dialogForgotPassword = false;
                        componentData.showMessage(componentData.label('sendEmailTitle'),
                            componentData.label('sendEmail', { email: componentData.email }));
                    })
                    .catch(function(error) {
                        console.log(error);
                        componentData.errorMessage(error.response.data);
                        console.log(error);
                    });
            }
        }
    }
}