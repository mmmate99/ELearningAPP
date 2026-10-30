import api from './api';

export const fileService = {
  uploadFile: async (file: File, fileType: string = 'document'): Promise<any> => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('fileType', fileType);

    const response = await api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  deleteFile: async (filePath: string): Promise<void> => {
    await api.delete(`/files/${encodeURIComponent(filePath)}`);
  },

  processExternalVideo: async (videoUrl: string): Promise<string> => {
    const response = await api.post('/files/external-video', { videoUrl });
    return response.data.embedUrl;
  },

  getAllowedExtensions: async (fileType: string): Promise<string[]> => {
    const response = await api.get(`/files/allowed-extensions/${fileType}`);
    return response.data;
  },
};