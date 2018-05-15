import "babel-polyfill";
import Vue from 'vue';
import Vuetify from 'vuetify';
import MultiLanguage from 'vue-multilanguage';
import VeeValidate from 'vee-validate';

Vue.use(VeeValidate);

Vue.use(MultiLanguage,
    {
        default: 'ru',
        ru: {
            pset: '.',
            required: 'Обязательное поле',
            toStart: 'Для начала или продолжения регистрации, пожалуйста, введите адрес Вашей электронной почты (на который Вы получили приглашение) и пароль',
            resetIt: 'Если Вы еще не установили пароль или хотите изменить его, пожалуйста, перейдите',
            resetItLink: 'по ссылке',
            email: 'Email',
            password: 'Пароль',
            login: 'Войти',
            startRegistration: 'Зарегистрироваться',
            dataError: 'Ошибка заполнения данных',
            loginPasswordError: 'Email или пароль введен неверно',
            toSetPass: 'Чтобы создать пароль, пожалуйста, введите адрес электронной почты, на который Вы получили приглашение',
            toSetTitle: 'Забыли пароль?',
            toSet: 'Пожалуйста, введите адрес Вашей электронной почты (на который Вы получили приглашение), нажмите кнопку «ОТПРАВИТЬ», и Вы получите электронное письмо со ссылкой на изменение пароля',
            receiveInsructions: 'Если после нажатия кнопки «ОТПРАВИТЬ» Вы не получите электронное письмо от «Aurora Prize Guest Management», проверьте, пожалуйста, папки «Нежелательная почта» и «Вся почта» своего ящика электронной почты. Если Вы все же не видите наше письмо, свяжитесь, пожалуйста, с CRM-менеджером Зарой Газарян по адресу: ',
            receiveInstructions: 'Отправить',
            receiveInsructions2: '',
            close: 'Закрыть',
            sendEmailTitle: 'Письмо отправлено',
            sendEmail: 'Письмо со ссылкой на изменение пароля направлено по адресу: {email}.',
            errorTitle: 'Ошибка',
            guestNotFound: 'Гость с адресом электронной почты {val} не найден.',
            guestRegistred: 'Этот Гость уже зарегистрирован с адресом {val}. Пожалуйста, используйте этот адрес для доступа к регистрационной форме.',
            emailError: 'Неверный адрес электронной почты',
            error: 'Ошибка изменения пароля'
        }
    });


Vue.use(Vuetify);

new Vue({
    el: '#app-account',
    render: function (h) {
        return h(require('./components/Account/Account.vue.html'));
    }
});
