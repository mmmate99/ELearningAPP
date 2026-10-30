export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface ErrorResponse {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}

export interface SelectOption {
  value: string | number;
  label: string;
}

export interface BreadcrumbItem {
  label: string;
  path: string;
}

export interface Notification {
  id: string;
  type: 'info' | 'success' | 'warning' | 'error';
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
  data?: any;
}