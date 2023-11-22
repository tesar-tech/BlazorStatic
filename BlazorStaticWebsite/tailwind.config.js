const colors = require('tailwindcss/colors');
const vidaloca = {
    '50': '#f4f9ec',
    '100': '#e7f2d5',
    '200': '#d0e7af',
    '300': '#b1d680',
    '400': '#95c358',
    '500': '#76a83a',
    '600': '#62902e',
    DEFAULT: '#62902e',
    '700': '#466625',
    '800': '#3a5222',
    '900': '#334720',
    '950': '#19260d',
};

module.exports = {
    content:
        [
            './Components/**/*.razor',
        ],
    theme: {
        extend: {
            fontFamily: {
                'tomorrow': ['Tomorrow', 'sans'],
            },
            colors: {
               vidaloca,
                primary:vidaloca,

            },
            typography: (theme) => ({
                DEFAULT: {
                    css: {
                        // 'font-family': `${theme('fontFamily.tomorrow')}`,
                        h1: {color: vidaloca['600'],},
                        h2: {color: vidaloca['400'],},
                        h3: {color: vidaloca['300'],},
                        h4: {color: vidaloca['400'],},
                    }
                },
            }),
           
        },},
    plugins: [require('@tailwindcss/typography')],
}
