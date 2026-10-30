import api from './api';
import { Lesson } from '../types/course';

export const lessonService = {
  getLessonById: async (id: number): Promise<Lesson> => {
    const response = await api.get<Lesson>(`/lessons/${id}`);
    return response.data;
  },

  createLesson: async (lessonData: any): Promise<Lesson> => {
    const response = await api.post<Lesson>('/lessons', lessonData);
    return response.data;
  },

  updateLesson: async (id: number, lessonData: any): Promise<Lesson> => {
    const response = await api.put<Lesson>(`/lessons/${id}`, lessonData);
    return response.data;
  },

  deleteLesson: async (id: number): Promise<void> => {
    await api.delete(`/lessons/${id}`);
  },

  updateProgress: async (id: number, progressData: any): Promise<any> => {
    const response = await api.post(`/lessons/${id}/progress`, progressData);
    return response.data;
  },

  markAsCompleted: async (id: number): Promise<void> => {
    await api.post(`/lessons/${id}/complete`);
  },

  getRecentLessons: async (): Promise<Lesson[]> => {
    const response = await api.get<Lesson[]>('/lessons/recent');
    return response.data;
  },
};