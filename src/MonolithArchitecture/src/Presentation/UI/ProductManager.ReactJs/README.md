# ProductManager UI (React)

Quick guide to run the Ant Design Pro UI locally and connect it to the ProductManager API.

Prerequisites
- Node.js (>=18 recommended)
- npm or yarn

Run locally (development)

```bash
# Install deps
npm install

# Start dev server (default port 8000)
npm start
```

Proxy (development)
- The app uses `config/proxy.ts` to forward `/api` requests to `http://localhost:5000` by default.

Product Pages
- The UI includes a Product Management section at `/products` with list, create, edit and detail views.
- The Product list supports client-side searching and CSV export.

Health Checks
- Ensure the API is running and health endpoints are available at `/health`, `/health/ready`, and `/health/live` for monitoring and Kubernetes probes.

Tests

```bash
# Run Jest tests
npm test
```

Notes
- Some project test utilities depend on `@umijs/max/test` and related packages — if tests fail to start, run `npm install` and ensure the workspace dependencies are installed.
# Ant Design Pro

This project is initialized with [Ant Design Pro](https://pro.ant.design). Follow is the quick guide for how to use.

## Environment Prepare

Install `node_modules`:

```bash
npm install
```

or

```bash
yarn
```

## Provided Scripts

Ant Design Pro provides some useful script to help you quick start and build with web project, code style check and test.

Scripts provided in `package.json`. It's safe to modify or add additional script:

### Start project

```bash
npm start
```

### Build project

```bash
npm run build
```

### Check code style

```bash
npm run lint
```

You can also use script to auto fix some lint error:

```bash
npm run lint:fix
```

### Test code

```bash
npm test
```

## More

You can view full document on our [official website](https://pro.ant.design). And welcome any feedback in our [github](https://github.com/ant-design/ant-design-pro).
