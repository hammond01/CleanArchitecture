import { Navigate, Route, Routes } from 'react-router-dom'
import { AppShell } from '@/widgets/layout/AppShell'

const Placeholder = ({ title, description }: { title: string; description: string }) => (
  <section className="rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
    <h2 className="text-2xl font-semibold text-ink">{title}</h2>
    <p className="mt-2 text-slate-600">{description}</p>
  </section>
)

export function AppRouter() {
  return (
    <Routes>
      <Route element={<AppShell />}>
        <Route
          path="/login"
          element={<Placeholder title="Login" description="Identity module presentation layer." />}
        />
        <Route
          path="/register"
          element={<Placeholder title="Register" description="Registration use-case entry point." />}
        />
        <Route
          path="/catalog/categories"
          element={<Placeholder title="Categories" description="Catalog categories management module." />}
        />
        <Route
          path="/catalog/products"
          element={<Placeholder title="Products" description="Catalog products management module." />}
        />
        <Route
          path="/auditing/logs"
          element={<Placeholder title="Audit Logs" description="Auditing read-only history module." />}
        />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Route>
    </Routes>
  )
}
