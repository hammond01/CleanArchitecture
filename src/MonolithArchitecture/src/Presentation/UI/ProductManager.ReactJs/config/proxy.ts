/**
 * @name Proxy configuration
 * @see Proxy cannot take effect in production environment, so there is no production environment configuration here
 * -------------------------------
 * The agent cannot take effect in the production environment
 * so there is no configuration of the production environment
 * For details, please see
 * https://pro.ant.design/docs/deploy
 *
 * @doc https://umijs.org/docs/guides/proxy
 */
export default {
  // If you need to customize the local development server, please uncomment and adjust as needed
  dev: {
    // localhost:8000/api/** -> http://localhost:5000/api/**
    '/api/': {
      // The address to proxy to
      target: 'http://localhost:5000',
      // Configuring this allows proxying from http to https
      // Features that depend on origin may need this, such as cookies
      changeOrigin: true,
    },
  },
  /**
   * @name Detailed proxy configuration
   * @doc https://github.com/chimurai/http-proxy-middleware
   */
  test: {
    // localhost:8000/api/** -> https://preview.pro.ant.design/api/**
    '/api/': {
      target: 'https://proapi.azurewebsites.net',
      changeOrigin: true,
      pathRewrite: { '^': '' },
    },
  },
  pre: {
    '/api/': {
      target: 'your pre url',
      changeOrigin: true,
      pathRewrite: { '^': '' },
    },
  },
};
