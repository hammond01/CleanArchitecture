import { productApi, categoryApi, supplierApi, customerApi, orderApi } from '@/services/api';
import type { ActivitiesType, AnalysisData, NoticeType } from './data';

export async function queryProjectNotice(): Promise<{ data: NoticeType[] }> {
  // Return mock notices for now - can be replaced with real API later
  return {
    data: [
      {
        id: '1',
        title: 'Welcome to ProductManager',
        logo: '',
        description: 'Your enterprise product management system is ready',
        updatedAt: new Date().toISOString(),
        member: 'Product Team',
        href: '',
        memberLink: '',
      },
      {
        id: '2',
        title: 'API Documentation',
        logo: '',
        description: 'Check out the comprehensive API documentation',
        updatedAt: new Date().toISOString(),
        member: 'Docs Team',
        href: '',
        memberLink: '',
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
        updatedAt: '2025-12-12T10:00:00Z',
        user: {
          name: 'Admin',
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png',
        },
        group: { name: 'Product Management', link: '' },
        project: { name: 'Product Management', link: '' },
        template: '',
      },
      {
        id: '2',
        updatedAt: '2025-12-12T09:30:00Z',
        user: {
          name: 'System',
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png',
        },
        group: { name: 'Health', link: '' },
        project: { name: 'Health Check', link: '' },
        template: '',
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
        visitData: [],
        visitData2: [],
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
        searchData: [],
        offlineData: [],
        offlineChartData: [],
        salesTypeData: [],
        salesTypeDataOnline: [],
        salesTypeDataOffline: [],
        radarData: [
          { name: 'Products', label: 'Products', value: productCount },
          { name: 'Categories', label: 'Categories', value: categoryCount },
          { name: 'Suppliers', label: 'Suppliers', value: supplierCount },
          { name: 'Customers', label: 'Customers', value: customerCount },
          { name: 'Orders', label: 'Orders', value: orderCount },
        ],
      },
    };  } catch (error) {
    console.error('Failed to fetch dashboard data:', error);
    // Return mock data as fallback
    return {
      data: {
        visitData: [],
        visitData2: [],
        salesData: [],
        searchData: [],
        offlineData: [],
        offlineChartData: [],
        salesTypeData: [],
        salesTypeDataOnline: [],
        salesTypeDataOffline: [],
        radarData: [],
      },
    };
  }
}
