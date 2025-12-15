import { productApi, categoryApi, supplierApi, customerApi, orderApi } from '@/services/api';
import type { ActivitiesType, AnalysisData, NoticeType } from './data';

export async function queryProjectNotice(): Promise<{ data: NoticeType[] }> {
  // Return mock notices for now - can be replaced with real API later
  return {
    data: [
      {
        id: '1',
        title: 'Welcome to ProductManager',
        description: 'Your enterprise product management system is ready',
        extra: 'Now',
        status: 'todo',
      },
      {
        id: '2',
        title: 'API Documentation',
        description: 'Check out the comprehensive API documentation',
        extra: '1 hour ago',
        status: 'urgent',
      },
    ],
  };
}

export async function queryActivities(): Promise<{ data: ActivitiesType[] }> {
  // Return mock activities for now
  return {
    data: [
      {
        id: '1',
        user: {
          nickname: 'Admin',
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png',
        },
        project: {
          name: 'Product Management',
          action: 'created',
        },
        time: '2025-12-12 10:00:00',
      },
      {
        id: '2',
        user: {
          nickname: 'System',
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png',
        },
        project: {
          name: 'Health Check',
          action: 'completed',
        },
        time: '2025-12-12 09:30:00',
      },
    ],
  };
}

export async function fakeChartData(): Promise<{ data: AnalysisData }> {
  try {
    // Get real data from APIs
    const [products, categories, suppliers, customers, orders] = await Promise.all([
      productApi.getAll(),
      categoryApi.getAll(),
      supplierApi.getAll(),
      customerApi.getAll(),
      orderApi.getAll(),
    ]);

    const productCount = products.data?.length || 0;
    const categoryCount = categories.data?.length || 0;
    const supplierCount = suppliers.data?.length || 0;
    const customerCount = customers.data?.length || 0;
    const orderCount = orders.data?.length || 0;

    return {
      data: {
        users: customerCount,
        categories: categoryCount,
        products: productCount,
        suppliers: supplierCount,
        orders: orderCount,
        salesData: [
          { x: '1月', y: Math.floor(Math.random() * 1000) },
          { x: '2月', y: Math.floor(Math.random() * 1000) },
          { x: '3月', y: Math.floor(Math.random() * 1000) },
          { x: '4月', y: Math.floor(Math.random() * 1000) },
          { x: '5月', y: Math.floor(Math.random() * 1000) },
          { x: '6月', y: Math.floor(Math.random() * 1000) },
          { x: '7月', y: Math.floor(Math.random() * 1000) },
          { x: '8月', y: Math.floor(Math.random() * 1000) },
          { x: '9月', y: Math.floor(Math.random() * 1000) },
          { x: '10月', y: Math.floor(Math.random() * 1000) },
          { x: '11月', y: Math.floor(Math.random() * 1000) },
          { x: '12月', y: Math.floor(Math.random() * 1000) },
        ],
        cpu: {
          used: 75,
          total: 100,
        },
        radarData: [
          { name: 'Products', value: productCount },
          { name: 'Categories', value: categoryCount },
          { name: 'Suppliers', value: supplierCount },
          { name: 'Customers', value: customerCount },
          { name: 'Orders', value: orderCount },
        ],
      },
    };
  } catch (error) {
    console.error('Failed to fetch dashboard data:', error);
    // Return mock data as fallback
    return {
      data: {
        users: 0,
        categories: 0,
        products: 0,
        suppliers: 0,
        orders: 0,
        salesData: [],
        cpu: { used: 0, total: 100 },
        radarData: [],
      },
    };
  }
}
