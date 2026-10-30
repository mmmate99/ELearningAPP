export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  role: UserRole;
  profileImageUrl?: string;
  bio?: string; // Add this
  createdAt: string;
}

export enum UserRole {
  Student = 'Student',
  Teacher = 'Teacher',
  Admin = 'Admin'
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  user: User;
}