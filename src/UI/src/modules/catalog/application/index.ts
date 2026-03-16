export const catalogQueryKeys = {
  categories: (searchTerm = '') => ['catalog', 'categories', searchTerm] as const,
  products: (categoryId = '', discontinued = 'all') =>
    ['catalog', 'products', categoryId, discontinued] as const,
}
