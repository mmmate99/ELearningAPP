import api from './api';
import { Module } from '../types/course';

export const moduleService = {
  getModuleById: async (id: number): Promise<Module> => {
    const response = await api.get<Module>(`/modules/${id}`);
    return response.data;
  },

  createModule: async (moduleData: any): Promise<Module> => {
    const response = await api.post<Module>('/modules', moduleData);
    return response.data;
  },

  updateModule: async (id: number, moduleData: any): Promise<Module> => {
    const response = await api.put<Module>(`/modules/${id}`, moduleData);
    return response.data;
  },

  deleteModule: async (id: number): Promise<void> => {
    await api.delete(`/modules/${id}`);
  },
};