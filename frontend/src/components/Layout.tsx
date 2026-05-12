import { ReactNode } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from '@/contexts/AuthContext'
import { cn } from '@/lib/utils'
import {
  LayoutDashboard, Users, GraduationCap, BookOpen, ClipboardList, Calendar,
  DollarSign, LogOut, Menu, X, School,
} from 'lucide-react'
import { useState } from 'react'

const navItems = [
  { href: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { href: '/students', label: 'Alunos', icon: Users },
  { href: '/teachers', label: 'Professores', icon: GraduationCap },
  { href: '/classes', label: 'Turmas', icon: School },
  { href: '/subjects', label: 'Disciplinas', icon: BookOpen },
  { href: '/enrollments', label: 'Matrículas', icon: Users },
  { href: '/school-years', label: 'Anos Letivos', icon: Calendar },
  { href: '/grades', label: 'Notas', icon: ClipboardList },
  { href: '/financial', label: 'Financeiro', icon: DollarSign },
]

export function Layout({ children }: { children: ReactNode }) {
  const { user, logout } = useAuth()
  const location = useLocation()
  const navigate = useNavigate()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  const handleLogout = () => { logout(); navigate('/login') }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Mobile header */}
      <header className="lg:hidden flex items-center justify-between p-4 bg-white border-b">
        <button onClick={() => setSidebarOpen(!sidebarOpen)}>
          {sidebarOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
        </button>
        <h1 className="font-bold text-lg text-primary">EduCore</h1>
        <div className="w-6" />
      </header>

      {/* Sidebar */}
      <aside className={cn(
        "fixed inset-y-0 left-0 z-50 w-64 bg-white border-r transform transition-transform lg:translate-x-0",
        sidebarOpen ? "translate-x-0" : "-translate-x-full"
      )}>
        <div className="p-6 border-b">
          <h1 className="text-2xl font-bold text-primary">EduCore</h1>
          <p className="text-sm text-gray-500 mt-1">ERP Escolar</p>
        </div>
        <nav className="p-4 space-y-1">
          {navItems.map(item => (
            <Link
              key={item.href}
              to={item.href}
              onClick={() => setSidebarOpen(false)}
              className={cn(
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors",
                location.pathname === item.href
                  ? "bg-primary text-white"
                  : "text-gray-600 hover:bg-gray-100"
              )}
            >
              <item.icon className="w-5 h-5" />
              {item.label}
            </Link>
          ))}
        </nav>
        <div className="absolute bottom-0 left-0 right-0 p-4 border-t">
          <div className="flex items-center gap-3 mb-3">
            <div className="w-8 h-8 rounded-full bg-primary flex items-center justify-center text-white text-sm font-medium">
              {user?.name?.[0]?.toUpperCase()}
            </div>
            <div>
              <p className="text-sm font-medium">{user?.name}</p>
              <p className="text-xs text-gray-500 capitalize">{user?.role}</p>
            </div>
          </div>
          <button onClick={handleLogout} className="flex items-center gap-2 text-sm text-gray-500 hover:text-red-600 w-full">
            <LogOut className="w-4 h-4" /> Sair
          </button>
        </div>
      </aside>

      {/* Overlay */}
      {sidebarOpen && <div className="fixed inset-0 bg-black/50 z-40 lg:hidden" onClick={() => setSidebarOpen(false)} />}

      {/* Main */}
      <main className="lg:ml-64 p-6">
        {children}
      </main>
    </div>
  )
}
