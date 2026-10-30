import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Notification } from '../../types/common';

interface UIState {
  notifications: Notification[];
  sidebarOpen: boolean;
  themeMode: 'light' | 'dark';
  isLoading: boolean;
  dialog: {
    open: boolean;
    title: string;
    content: string;
    onConfirm?: () => void;
  };
}

const initialState: UIState = {
  notifications: [],
  sidebarOpen: true,
  themeMode: 'light',
  isLoading: false,
  dialog: {
    open: false,
    title: '',
    content: '',
  },
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    toggleSidebar: (state) => {
      state.sidebarOpen = !state.sidebarOpen;
    },
    setThemeMode: (state, action: PayloadAction<'light' | 'dark'>) => {
      state.themeMode = action.payload;
    },
    addNotification: (state, action: PayloadAction<Omit<Notification, 'id' | 'timestamp' | 'read'>>) => {
      const notification: Notification = {
        id: Date.now().toString(),
        timestamp: new Date(),
        read: false,
        ...action.payload,
      };
      state.notifications.unshift(notification);
    },
    markNotificationAsRead: (state, action: PayloadAction<string>) => {
      const notification = state.notifications.find(n => n.id === action.payload);
      if (notification) {
        notification.read = true;
      }
    },
    removeNotification: (state, action: PayloadAction<string>) => {
      state.notifications = state.notifications.filter(n => n.id !== action.payload);
    },
    clearNotifications: (state) => {
      state.notifications = [];
    },
    showDialog: (state, action: PayloadAction<{ title: string; content: string; onConfirm?: () => void }>) => {
      state.dialog = {
        open: true,
        ...action.payload,
      };
    },
    hideDialog: (state) => {
      state.dialog.open = false;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
  },
});

export const {
  toggleSidebar,
  setThemeMode,
  addNotification,
  markNotificationAsRead,
  removeNotification,
  clearNotifications,
  showDialog,
  hideDialog,
  setLoading,
} = uiSlice.actions;

export default uiSlice.reducer;