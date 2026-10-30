import api from './api';
import { ForumPost, CreateForumPost } from '../types/forum';

export const forumService = {
  getCoursePosts: async (courseId: number): Promise<ForumPost[]> => {
    const response = await api.get<ForumPost[]>(`/forum/courses/${courseId}`);
    return response.data;
  },

  getPostById: async (postId: number): Promise<ForumPost> => {
    const response = await api.get<ForumPost>(`/forum/${postId}`);
    return response.data;
  },

  createPost: async (postData: CreateForumPost): Promise<ForumPost> => {
    const response = await api.post<ForumPost>('/forum', postData);
    return response.data;
  },

  updatePost: async (postId: number, postData: Partial<CreateForumPost>): Promise<ForumPost> => {
    const response = await api.put<ForumPost>(`/forum/${postId}`, postData);
    return response.data;
  },

  deletePost: async (postId: number): Promise<void> => {
    await api.delete(`/forum/${postId}`);
  },

  togglePinPost: async (postId: number): Promise<void> => {
    await api.post(`/forum/${postId}/pin`);
  },

  searchPosts: async (query?: string, courseId?: number): Promise<ForumPost[]> => {
    const response = await api.get<ForumPost[]>('/forum/search', {
      params: { q: query, courseId }
    });
    return response.data;
  },

  getMyPosts: async (): Promise<ForumPost[]> => {
    const response = await api.get<ForumPost[]>('/forum/my-posts');
    return response.data;
  },
};