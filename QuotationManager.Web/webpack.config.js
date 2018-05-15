const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const bundleOutputDir = './wwwroot/dist';

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);

    const bundleOutputDir = './wwwroot/dist';
    return [{
        stats: { modules: false },
        context: __dirname,
        resolve: { extensions: [ '.js' ] },
        entry: {
            'main': './ClientApp/quota.js',
            'account': './ClientApp/account.js'
            //'reset': './ClientApp/reset.js'
        },
        module: {
            rules: [
                {
                    test: /\.vue\.html$/, include: /ClientApp/, loader: 'vue-loader',
                    options: { loaders: { js: 'babel-loader' } }
                },
                { test: /\.js$/, exclude: /node_modules/, use: ['babel-loader'] },
                { test: /\.css$/, use: isDevBuild ? [ 'style-loader', 'css-loader' ] : ExtractTextPlugin.extract({ use: 'css-loader?minimize' }) },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
            ]
        },
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: '[name].js',
            publicPath: '/dist/'
        },
        plugins: [
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? 'development' : 'production')
                }
            }),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            })
        ].concat(isDevBuild ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(bundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            })
        ] : [
            // Plugins that apply in production builds only
            //new webpack.optimize.UglifyJsPlugin()
        ])
    }];
};
