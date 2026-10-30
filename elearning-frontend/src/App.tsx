import React, {useEffect} from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store/store';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Toaster } from 'react-hot-toast';
import { useSignalR } from './hooks/useSignalR';

// Layout
import Layout from './components/layout/Layout';

// Pages
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import Dashboard from './pages/Dashboard';
import CourseList from './pages/courses/CourseList';
import CourseDetails from './pages/courses/CourseDetails';
import MyCourses from './pages/courses/MyCourses';
import TeachingCourses from './pages/courses/TeachingCourses';
import LessonView from './pages/courses/LessonView';
import QuizList from './pages/quiz/QuizList';
import QuizView from './pages/quiz/QuizView';
import ForumList from './pages/forum/ForumList';
import ForumPost from './pages/forum/ForumPost';
import Profile from './pages/Profile';
import Settings from './pages/Settings';

// Protected Route
import ProtectedRoute from './components/common/ProtectedRoute';

// Theme
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
  },
});

const AppContent: React.FC = () =>{
  const {isConnected} = useSignalR();

  useEffect(() => {
    console.log('SignalR connection status:', isConnected);
  }, [isConnected]);
  return (
    <Router>
      {}
    </Router>
  );
};

const App: React.FC = () => {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Toaster position="top-right" />
        <AppContent />
        <Router>
          <Routes>
            {/* Public routes */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* Protected routes with layout */}
            <Route path="/" element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }>
              <Route index element={<Navigate to="/dashboard" />} />
              <Route path="dashboard" element={<Dashboard />} />
              <Route path="courses" element={<CourseList />} />
              <Route path="courses/:id" element={<CourseDetails />} />
              <Route path="courses/:courseId/lessons/:lessonId" element={<LessonView />} />
              <Route path="my-courses" element={<MyCourses />} />
              <Route path="teaching" element={<TeachingCourses />} />
              <Route path="quizzes" element={<QuizList />} />
              <Route path="quizzes/:id" element={<QuizView />} />
              <Route path="forum" element={<ForumList />} />
              <Route path="forum/:id" element={<ForumPost />} />
              <Route path="profile" element={<Profile />} />
              <Route path="settings" element={<Settings />} />
            </Route>

            {/* Catch all */}
            <Route path="*" element={<Navigate to="/dashboard" />} />
          </Routes>
        </Router>
      </ThemeProvider>
    </Provider>
  );
};

export default App;