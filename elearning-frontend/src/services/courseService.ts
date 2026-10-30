import api from './api';
import { Course, CourseDetails, CreateCourseRequest, /*Module,*/ Lesson } from '../types/course';

export const courseService = {
  // Kurzusok
  getAllCourses: async (): Promise<Course[]> => {
    const response = await api.get<Course[]>('/courses');
    return response.data;
  },

  getCourseById: async (id: number): Promise<Course> => {
    const response = await api.get<Course>(`/courses/${id}`);
    return response.data;
  },

  getCourseDetails: async (id: number): Promise<CourseDetails> => {
    const response = await api.get<CourseDetails>(`/courses/${id}/details`);
    return response.data;
  },

  createCourse: async (courseData: CreateCourseRequest): Promise<Course> => {
    const response = await api.post<Course>('/courses', courseData);
    return response.data;
  },

  updateCourse: async (id: number, courseData: Partial<CreateCourseRequest>): Promise<Course> => {
    const response = await api.put<Course>(`/courses/${id}`, courseData);
    return response.data;
  },

  deleteCourse: async (id: number): Promise<void> => {
    await api.delete(`/courses/${id}`);
  },

  enrollInCourse: async (id: number): Promise<void> => {
    await api.post(`/courses/${id}/enroll`);
  },

  unenrollFromCourse: async (id: number): Promise<void> => {
    await api.post(`/courses/${id}/unenroll`);
  },

  rateCourse: async (id: number, rating: number, review?: string): Promise<void> => {
    await api.post(`/courses/${id}/rate`, { rating, review });
  },

  // Modulok és leckék
  getLessonWithProgress: async (id: number): Promise<Lesson> => {
    const response = await api.get<Lesson>(`/lessons/${id}`);
    return response.data;
  },

  updateLessonProgress: async (id: number, progressData: any): Promise<any> => {
    const response = await api.post(`/lessons/${id}/progress`, progressData);
    return response.data;
  },

  markLessonAsCompleted: async (id: number): Promise<void> => {
    await api.post(`/lessons/${id}/complete`);
  },

  // Saját kurzusok
  getMyCourses: async (): Promise<Course[]> => {
    const response = await api.get<Course[]>('/courses/my-courses');
    return response.data;
  },

  getTeachingCourses: async (): Promise<Course[]> => {
    const response = await api.get<Course[]>('/courses/teaching');
    return response.data;
  },

  // Keresés
  searchCourses: async (query: string, page = 1, pageSize = 10): Promise<Course[]> => {
    const response = await api.get<Course[]>('/courses/search', {
      params: { q: query, page, pageSize }
    });
    return response.data;
  },
};