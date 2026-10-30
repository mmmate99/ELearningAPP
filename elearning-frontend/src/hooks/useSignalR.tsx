import { useEffect, useRef } from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useAppSelector, useAppDispatch } from '../store/hooks';
import { addNotification } from '../features/ui/uiSlice';
import toast from 'react-hot-toast';

const SIGNALR_URL = import.meta.env.VITE_SIGNALR_URL || 'http://localhost:5001/notificationHub';

export const useSignalR = () => {
  const connectionRef = useRef<HubConnection | null>(null);
  const { token, user } = useAppSelector((state) => state.auth);
  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!token || !user) return;

    const createConnection = async () => {
      try {
        const connection = new HubConnectionBuilder()
          .withUrl(SIGNALR_URL, {
            accessTokenFactory: () => token,
          })
          .configureLogging(LogLevel.Information)
          .withAutomaticReconnect()
          .build();

        // Register handlers
        connection.on('ReceiveNotification', (notification) => {
          dispatch(addNotification({
            type: notification.Type || 'info',
            title: notification.Title || 'Értesítés',
            message: notification.Message,
            data: notification.Data,
          }));
          
          toast(notification.Message, {
            icon: '🔔',
          });
        });

        connection.on('ReceiveCourseNotification', (notification) => {
          dispatch(addNotification({
            type: 'info',
            title: 'Kurzus értesítés',
            message: notification.Message,
            data: notification.Data,
          }));
          
          toast(notification.Message, {
            icon: '📚',
          });
        });

        connection.on('ReceiveCourseMessage', (data) => {
          console.log('Course message received:', data);
        });

        connection.on('UserConnected', (data) => {
          console.log('User connected:', data);
        });

        connection.onreconnecting(() => {
          console.log('SignalR reconnecting...');
        });

        connection.onreconnected(() => {
          console.log('SignalR reconnected');
          toast.success('Kapcsolat helyreállt');
        });

        connection.onclose(() => {
          console.log('SignalR connection closed');
        });

        await connection.start();
        connectionRef.current = connection;

        console.log('SignalR connected');
      } catch (error) {
        console.error('SignalR connection error:', error);
        toast.error('Nem sikerült csatlakozni az értesítési szolgáltatáshoz');
      }
    };

    createConnection();

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, [token, user, dispatch]);

  const sendMessage = async (courseId: number, message: string) => {
    if (connectionRef.current?.state === 'Connected') {
      try {
        await connectionRef.current.invoke('SendMessageToCourse', courseId, message);
      } catch (error) {
        console.error('Error sending message:', error);
      }
    }
  };

  const sendNotification = async (userId: string, message: string, type: string = 'info') => {
    if (connectionRef.current?.state === 'Connected') {
      try {
        await connectionRef.current.invoke('SendNotificationToUser', userId, message, type);
      } catch (error) {
        console.error('Error sending notification:', error);
      }
    }
  };

  const joinCourseGroup = async (courseId: number) => {
    if (connectionRef.current?.state === 'Connected') {
      try {
        await connectionRef.current.invoke('JoinCourseGroup', courseId);
      } catch (error) {
        console.error('Error joining course group:', error);
      }
    }
  };

  const leaveCourseGroup = async (courseId: number) => {
    if (connectionRef.current?.state === 'Connected') {
      try {
        await connectionRef.current.invoke('LeaveCourseGroup', courseId);
      } catch (error) {
        console.error('Error leaving course group:', error);
      }
    }
  };

  return {
    sendMessage,
    sendNotification,
    joinCourseGroup,
    leaveCourseGroup,
    connection: connectionRef.current,
    isConnected: connectionRef.current?.state === 'Connected',
  };
};