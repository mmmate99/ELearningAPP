import api from './api';

export const progressService = {
  getCourseProgress: async (courseId: number): Promise<any> => {
    const response = await api.get(`/courses/${courseId}/progress`);
    return response.data;
  },

  updateLessonProgress: async (lessonId: number, data: {
    timeSpent?: number;
    currentTime?: number;
    isCompleted?: boolean;
  }): Promise<any> => {
    const response = await api.post(`/lessons/${lessonId}/progress`, data);
    return response.data;
  },

  getRecentLessons: async (): Promise<any[]> => {
    const response = await api.get('/lessons/recent');
    return response.data;
  },
};