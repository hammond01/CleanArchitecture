// API Services for ProductManager Backend
// This file contains all API calls to the .NET backend

export interface ApiResponse<T = any> {
  statusCode: number;
  message: string;
  data?: T;
  errors?: string[];
}

export interface Product {
  id: string;
  productName: string;
  supplierId?: string;
  categoryId?: string;
  quantityPerUnit?: string;
  unitPrice?: number;
  unitsInStock?: number;
  unitsOnOrder?: number;
  reorderLevel?: number;
  discontinued: boolean;
}

export interface Category {
  id: string;
  categoryName: string;
  description?: string;
  picture?: string;
}

export interface Supplier {
  id: string;
  companyName: string;
  contactName?: string;
  contactTitle?: string;
  address?: string;
  city?: string;
  region?: string;
  postalCode?: string;
  country?: string;
  phone?: string;
  fax?: string;
  homePage?: string;
}

export interface Customer {
  id: string;
  companyName: string;
  contactName?: string;
  contactTitle?: string;
  address?: string;
  city?: string;
  region?: string;
  postalCode?: string;
  country?: string;
  phone?: string;
  fax?: string;
}

export interface Employee {
  id: string;
  lastName: string;
  firstName: string;
  title?: string;
  titleOfCourtesy?: string;
  birthDate?: string;
  hireDate?: string;
  address?: string;
  city?: string;
  region?: string;
  postalCode?: string;
  country?: string;
  homePhone?: string;
  extension?: string;
  photo?: string;
  notes?: string;
  reportsTo?: string;
  photoPath?: string;
}

export interface Order {
  id: string;
  customerId?: string;
  employeeId?: string;
  orderDate?: string;
  requiredDate?: string;
  shippedDate?: string;
  shipVia?: number;
  freight?: number;
  shipName?: string;
  shipAddress?: string;
  shipCity?: string;
  shipRegion?: string;
  shipPostalCode?: string;
  shipCountry?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
}

// Product API
export const productApi = {
  // Get all products
  getAll: () => request.get<ApiResponse<Product[]>>('/api/v1/products'),

  // Get product by ID
  getById: (id: string) => request.get<ApiResponse<Product>>(`/api/v1/products/${id}`),

  // Create product
  create: (product: Omit<Product, 'id'>) =>
    request.post<ApiResponse<Product>>('/api/v1/products', product),

  // Update product
  update: (id: string, product: Partial<Product>) =>
    request.put<ApiResponse<Product>>(`/api/v1/products/${id}`, product),

  // Delete product
  delete: (id: string) => request.delete<ApiResponse<Product>>(`/api/v1/products/${id}`),
};

// Category API
export const categoryApi = {
  getAll: () => request.get<ApiResponse<Category[]>>('/api/v1/categories'),
  getById: (id: string) => request.get<ApiResponse<Category>>(`/api/v1/categories/${id}`),
  create: (category: Omit<Category, 'id'>) =>
    request.post<ApiResponse<Category>>('/api/v1/categories', category),
  update: (id: string, category: Partial<Category>) =>
    request.put<ApiResponse<Category>>(`/api/v1/categories/${id}`, category),
  delete: (id: string) => request.delete<ApiResponse<Category>>(`/api/v1/categories/${id}`),
};

// Supplier API
export const supplierApi = {
  getAll: () => request.get<ApiResponse<Supplier[]>>('/api/v1/suppliers'),
  getById: (id: string) => request.get<ApiResponse<Supplier>>(`/api/v1/suppliers/${id}`),
  create: (supplier: Omit<Supplier, 'id'>) =>
    request.post<ApiResponse<Supplier>>('/api/v1/suppliers', supplier),
  update: (id: string, supplier: Partial<Supplier>) =>
    request.put<ApiResponse<Supplier>>(`/api/v1/suppliers/${id}`, supplier),
  delete: (id: string) => request.delete<ApiResponse<Supplier>>(`/api/v1/suppliers/${id}`),
};

// Customer API
export const customerApi = {
  getAll: () => request.get<ApiResponse<Customer[]>>('/api/v1/customers'),
  getById: (id: string) => request.get<ApiResponse<Customer>>(`/api/v1/customers/${id}`),
  create: (customer: Omit<Customer, 'id'>) =>
    request.post<ApiResponse<Customer>>('/api/v1/customers', customer),
  update: (id: string, customer: Partial<Customer>) =>
    request.put<ApiResponse<Customer>>(`/api/v1/customers/${id}`, customer),
  delete: (id: string) => request.delete<ApiResponse<Customer>>(`/api/v1/customers/${id}`),
};

// Employee API
export const employeeApi = {
  getAll: () => request.get<ApiResponse<Employee[]>>('/api/v1/employees'),
  getById: (id: string) => request.get<ApiResponse<Employee>>(`/api/v1/employees/${id}`),
  create: (employee: Omit<Employee, 'id'>) =>
    request.post<ApiResponse<Employee>>('/api/v1/employees', employee),
  update: (id: string, employee: Partial<Employee>) =>
    request.put<ApiResponse<Employee>>(`/api/v1/employees/${id}`, employee),
  delete: (id: string) => request.delete<ApiResponse<Employee>>(`/api/v1/employees/${id}`),
};

// Order API
export const orderApi = {
  getAll: () => request.get<ApiResponse<Order[]>>('/api/v1/orders'),
  getById: (id: string) => request.get<ApiResponse<Order>>(`/api/v1/orders/${id}`),
  create: (order: Omit<Order, 'id'>) =>
    request.post<ApiResponse<Order>>('/api/v1/orders', order),
  update: (id: string, order: Partial<Order>) =>
    request.put<ApiResponse<Order>>(`/api/v1/orders/${id}`, order),
  delete: (id: string) => request.delete<ApiResponse<Order>>(`/api/v1/orders/${id}`),
};

// Logs API
export interface ApiLog {
  id: string;
  method: string;
  path: string;
  statusCode: number;
  responseTimeMs?: number;
  userId?: string | null;
  clientIpAddress?: string | null;
  timestamp: string;
  queryString?: string | null;
  requestBody?: string | null;
  responseBody?: string | null;
}

export interface ActionLog {
  id: string;
  actionName: string;
  userId?: string | null;
  objectId?: string | null;
  log?: string | null;
  timestamp: string;
}

export interface LogStatistics {
  ApiLogs: {
    TotalRequests: number;
    SuccessfulRequests: number;
    ErrorRequests: number;
    AverageResponseTime: number;
  };
  ActionLogs: {
    TotalActions: number;
  };
  TopEndpoints: Array<{
    Endpoint: string;
    Count: number;
    AverageResponseTime: number;
  }>;
}

export const logsApi = {
  getApiLogs: (
    page = 1,
    pageSize = 50,
    userId?: string,
    fromDate?: string,
    toDate?: string,
  ) =>
    request.get<any>(
      `/api/v1/logs/api?page=${page}&pageSize=${pageSize}${userId ? `&userId=${encodeURIComponent(userId)}` : ''}${fromDate ? `&fromDate=${encodeURIComponent(fromDate)}` : ''}${toDate ? `&toDate=${encodeURIComponent(toDate)}` : ''}`,
    ),

  getActionLogs: (
    page = 1,
    pageSize = 50,
    userId?: string,
    actionName?: string,
    fromDate?: string,
    toDate?: string,
  ) =>
    request.get<any>(
      `/api/v1/logs/actions?page=${page}&pageSize=${pageSize}${userId ? `&userId=${encodeURIComponent(userId)}` : ''}${actionName ? `&actionName=${encodeURIComponent(actionName)}` : ''}${fromDate ? `&fromDate=${encodeURIComponent(fromDate)}` : ''}${toDate ? `&toDate=${encodeURIComponent(toDate)}` : ''}`,
    ),

  getStatistics: (fromDate?: string, toDate?: string) =>
    request.get<ApiResponse<LogStatistics>>(
      `/api/v1/logs/statistics${fromDate ? `?fromDate=${encodeURIComponent(fromDate)}` : ''}${toDate ? `${fromDate ? `&` : `?`}toDate=${encodeURIComponent(toDate)}` : ''}`,
    ),
};

// Authentication API
export const authApi = {
  login: (credentials: LoginRequest) =>
    request.post<ApiResponse<LoginResponse>>('/api/v1/identity/login', credentials),

  register: (userData: RegisterRequest) =>
    request.post<ApiResponse>('/api/v1/identity/register', userData),

  refreshToken: (refreshToken: string) =>
    request.post<ApiResponse<LoginResponse>>('/api/v1/identity/refresh', { refreshToken }),

  logout: () => request.post<ApiResponse>('/api/v1/identity/logout'),

  getProfile: () => request.get<ApiResponse>('/api/v1/identity/profile'),
};

// Health Check API
export const healthApi = {
  basic: () => request.get('/health'),
  ready: () => request.get('/health/ready'),
  live: () => request.get('/health/live'),
  detailed: () => request.get('/api/v1.0/health/detailed'),
};

// Generic request helper
const request = {
  get: <T>(url: string) => fetch(url).then(res => res.json()) as Promise<T>,
  post: <T>(url: string, data?: any) =>
    fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    }).then(res => res.json()) as Promise<T>,
  put: <T>(url: string, data?: any) =>
    fetch(url, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    }).then(res => res.json()) as Promise<T>,
  delete: <T>(url: string) =>
    fetch(url, { method: 'DELETE' }).then(res => res.json()) as Promise<T>,
};
