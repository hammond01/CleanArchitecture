import { NavLink, Outlet } from 'react-router-dom'

const links = [
  { to: '/login', label: 'Login' },
  { to: '/register', label: 'Register' },
  { to: '/catalog/categories', label: 'Categories' },
  { to: '/catalog/products', label: 'Products' },
  { to: '/auditing/logs', label: 'Audit Logs' },
]

export function AppShell() {
  return (
    <div className="min-h-screen">
      <header className="border-b border-slate-200 bg-white/80 backdrop-blur">
        <div className="mx-auto flex max-w-6xl flex-col gap-4 px-4 py-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between">
            <h1 className="text-lg font-bold tracking-tight text-ink">Clean Architecture UI</h1>
            <p className="text-sm text-slate-500">React + TS + Vite + Tailwind + TanStack Query</p>
          </div>
          <nav className="flex flex-wrap gap-2">
            {links.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                className={({ isActive }) =>
                  `rounded-full px-3 py-1.5 text-sm font-medium transition ${
                    isActive
                      ? 'bg-ink text-white'
                      : 'bg-slate-100 text-slate-600 hover:bg-slate-200 hover:text-slate-900'
                  }`
                }
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
        <Outlet />
      </main>
    </div>
  )
}
