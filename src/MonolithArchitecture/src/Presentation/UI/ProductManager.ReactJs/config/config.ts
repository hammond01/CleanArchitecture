// https://umijs.org/config/

import { defineConfig } from "@umijs/max";
import { join } from "node:path";
import defaultSettings from "./defaultSettings";
import proxy from "./proxy";

import routes from "./routes";

const { REACT_APP_ENV = "dev" } = process.env;

/**
 * @name Use public path
 * @description The path for deployment, configure this variable if deployed in a non-root directory
 * @doc https://umijs.org/docs/api/config#publicpath
 */
const PUBLIC_PATH: string = "/";

export default defineConfig({
  /**
   * @name Enable hash mode
   * @description Make the build artifacts contain hash suffixes. Usually used for incremental releases and to avoid browser cache loading.
   * @doc https://umijs.org/docs/api/config#hash
   */
  hash: true,

  publicPath: PUBLIC_PATH,

  /**
   * @name Compatibility settings
   * @description Setting ie11 may not be perfectly compatible, need to check all dependencies used
   * @doc https://umijs.org/docs/api/config#targets
   */
  // targets: {
  //   ie: 11,
  // },
  /**
   * @name Route configuration, files not introduced in routes will not be compiled
   * @description Only supports path, component, routes, redirect, wrappers, title configuration
   * @doc https://umijs.org/docs/guides/routes
   */
  // umi routes: https://umijs.org/docs/routing
  routes,
  /**
   * @name Theme configuration
   * @description Although called theme, it's actually just less variable settings
   * @doc antd theme settings https://ant.design/docs/react/customize-theme-cn
   * @doc umi theme config https://umijs.org/docs/api/config#theme
   */
  // theme: { '@primary-color': '#1DA57A' }
  /**
   * @name moment internationalization configuration
   * @description If there are no internationalization requirements, enabling this can reduce the js bundle size
   * @doc https://umijs.org/docs/api/config#ignoremomentlocale
   */
  ignoreMomentLocale: true,
  /**
   * @name Proxy configuration
   * @description Allows your local server to proxy to your server, so you can access server data
   * @see Note that proxy can only be used in local development, it cannot be used after build.
   * @doc Proxy introduction https://umijs.org/docs/guides/proxy
   * @doc Proxy configuration https://umijs.org/docs/api/config#proxy
   */
  proxy: proxy[REACT_APP_ENV as keyof typeof proxy],
  /**
   * @name Fast refresh configuration
   * @description A good hot update component that can retain state during updates
   */
  fastRefresh: true,
  //============== The following are all max plugin configurations ===============
  /**
   * @name Data flow plugin
   * @@doc https://umijs.org/docs/max/data-flow
   */
  model: {},
  /**
   * A global initial data flow that can be used to share data between plugins
   * @description Can be used to store some global data, such as user information, or some global state, global initial state is created at the very beginning of the entire Umi project.
   * @doc https://umijs.org/docs/max/data-flow#%E5%85%A8%E5%B1%80%E5%88%9D%E5%A7%8B%E7%8A%B6%E6%80%81
   */
  initialState: {},
  /**
   * @name layout plugin
   * @doc https://umijs.org/docs/max/layout-menu
   */
  title: "Ant Design Pro",
  layout: {
    locale: true,
    ...defaultSettings,
  },
  /**
   * @name moment2dayjs plugin
   * @description Replace moment in the project with dayjs
   * @doc https://umijs.org/docs/max/moment2dayjs
   */
  moment2dayjs: {
    preset: "antd",
    plugins: ["duration"],
  },
  /**
   * @name Internationalization plugin
   * @doc https://umijs.org/docs/max/i18n
   */
  locale: {
    // default zh-CN
    default: "en-US",
    antd: true,
    // default true, when it is true, will use `navigator.language` overwrite default
    baseNavigator: false,
  },
  /**
   * @name antd plugin
   * @description Built-in babel import plugin
   * @doc https://umijs.org/docs/max/antd#antd
   */
  antd: {
    appConfig: {},
    configProvider: {
      theme: {
        cssVar: true,
        token: {
          fontFamily: "AlibabaSans, sans-serif",
        },
      },
    },
  },
  /**
   * @name Network request configuration
   * @description It provides a unified network request and error handling solution based on axios and ahooks' useRequest.
   * @doc https://umijs.org/docs/max/request
   */
  request: {},
  /**
   * @name Access plugin
   * @description Permission plugin based on initialState, must enable initialState first
   * @doc https://umijs.org/docs/max/access
   */
  access: {},
  /**
   * @name Extra script in <head>
   * @description Configure extra script in <head>
   */
  headScripts: [
    // Solve the white screen problem on first load
    { src: join(PUBLIC_PATH, "scripts/loading.js"), async: true },
  ],
  //================ pro plugin configurations =================
  presets: ["umi-presets-pro"],
  /**
   * @name openAPI plugin configuration
   * @description Generate serve and mock based on openapi specification, can reduce a lot of boilerplate code
   * @doc https://pro.ant.design/zh-cn/docs/openapi/
   */
  openAPI: [
    {
      requestLibPath: "import { request } from '@umijs/max'",
      // 或者使用在线的版本
      // schemaPath: "https://gw.alipayobjects.com/os/antfincdn/M%24jrzTTYJN/oneapi.json"
      schemaPath: join(__dirname, "oneapi.json"),
      mock: false,
    },
    {
      requestLibPath: "import { request } from '@umijs/max'",
      schemaPath:
        "https://gw.alipayobjects.com/os/antfincdn/CA1dOm%2631B/openapi.json",
      projectName: "swagger",
    },
  ],
  mock: false, // Temporarily disabled to eliminate duplicate warnings
  /**
   * @name Whether to enable mako
   * @description Use mako for rapid development
   * @doc https://umijs.org/docs/api/config#mako
   */
  mako: {},
  esbuildMinifyIIFE: true,
  requestRecord: {},
  exportStatic: {},
  define: {
    "process.env.CI": process.env.CI,
  },
});
