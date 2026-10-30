interface EnvConfig {
  apiUrl: string;
  signalRUrl: string;
}

const getEnvConfig = (): EnvConfig => {
  const apiUrl = import.meta.env.VITE_API_URL;
  const signalRUrl = import.meta.env.VITE_SIGNALR_URL;

  if (!apiUrl) {
    console.warn('VITE_API_URL is not defined, using default');
  }

  if (!signalRUrl) {
    console.warn('VITE_SIGNALR_URL is not defined, using default');
  }

  return {
    apiUrl: apiUrl || 'http://localhost:5001/api',
    signalRUrl: signalRUrl || 'http://localhost:5001/notificationHub',
  };
};

export const env = getEnvConfig();