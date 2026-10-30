import api from './api';
import { Quiz, QuizAttempt, QuizResult } from '../types/quiz';

export const quizService = {
  getQuizById: async (id: number): Promise<Quiz> => {
    const response = await api.get<Quiz>(`/quizzes/${id}`);
    return response.data;
  },

  getQuizQuestions: async (id: number): Promise<any> => {
    const response = await api.get(`/quizzes/${id}/questions`);
    return response.data;
  },

  submitQuizAttempt: async (quizId: number, attempt: QuizAttempt): Promise<QuizResult> => {
    const response = await api.post<QuizResult>(`/quizzes/${quizId}/attempt`, attempt);
    return response.data;
  },

  getUserQuizAttempts: async (quizId: number): Promise<any[]> => {
    const response = await api.get(`/quizzes/${quizId}/attempts`);
    return response.data;
  },
};