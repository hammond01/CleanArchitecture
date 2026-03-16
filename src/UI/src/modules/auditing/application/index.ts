export const auditingQueryKeys = {
  logs: (pageNumber: number, pageSize: number) => ['auditing', 'logs', pageNumber, pageSize] as const,
}
