const API = import.meta.env.VITE_API_URL || '/api'

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token')
  const headers: Record<string, string> = { 'Content-Type': 'application/json' }
  if (token) headers['Authorization'] = `Bearer ${token}`

  const res = await fetch(`${API}${path}`, { ...options, headers })
  if (res.status === 401) {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    window.location.href = '/login'
    throw new Error('Não autorizado')
  }
  const data = await res.json()
  if (!res.ok) throw new Error(data.message || data.title || 'Erro na requisição')
  return data
}

export const api = {
  auth: {
    login: (email: string, password: string) =>
      apiFetch<{ token: string; name: string; role: string }>('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
      }),
    me: () => apiFetch<{ id: string; name: string; email: string; role: string }>('/auth/me'),
  },

  students: {
    list: () => apiFetch<any[]>('/students'),
    getById: (id: string) => apiFetch<any>(`/students/${id}`),
    create: (data: any) =>
      apiFetch<any>('/students', { method: 'POST', body: JSON.stringify(data) }),
    toggleStatus: (id: string) =>
      apiFetch<any>(`/students/${id}/toggle-status`, { method: 'PATCH' }),
  },

  academic: {
    classes: {
      list: () => apiFetch<any[]>('/academic/classes'),
      create: (data: { name: string; shift: string; room?: string }) =>
        apiFetch<any>('/academic/classes', { method: 'POST', body: JSON.stringify(data) }),
    },
    subjects: {
      list: () => apiFetch<any[]>('/academic/subjects'),
      create: (data: { name: string; code: string; workload: number }) =>
        apiFetch<any>('/academic/subjects', { method: 'POST', body: JSON.stringify(data) }),
    },
    assignTeacher: (data: { teacherId: string; subjectId: string; classId: string }) =>
      apiFetch<any>('/academic/assign-teacher', { method: 'POST', body: JSON.stringify(data) }),
    grades: {
      getByClass: (classId: string, bimester: number, year?: number) =>
        apiFetch<any[]>(`/academic/grades/${classId}?bimester=${bimester}&year=${year ?? new Date().getFullYear()}`),
      submit: (data: any) =>
        apiFetch<any>('/academic/grades/batch', { method: 'POST', body: JSON.stringify(data) }),
    },
    attendance: {
      submit: (data: any) =>
        apiFetch<any>('/academic/attendance/batch', { method: 'POST', body: JSON.stringify(data) }),
    },
  },
}
