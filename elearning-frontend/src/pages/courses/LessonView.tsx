// src/pages/courses/LessonView.tsx - TELJES VERZIÓ
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Grid, Alert } from '@mui/material';
import { lessonService } from '../../services/lessonService';
import { Lesson } from '../../types/course';
import {
  Container,
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  IconButton,
  LinearProgress,
  Breadcrumbs,
  Link,
} from '@mui/material';
import {
  PlayCircle as PlayCircleIcon,
  ArrowBack as ArrowBackIcon,
  ArrowForward as ArrowForwardIcon,
  CheckCircle as CheckCircleIcon,
  Description as DescriptionIcon,
  Home as HomeIcon,
  School as SchoolIcon,
  Description,
} from '@mui/icons-material';
import toast from 'react-hot-toast';

const LessonView: React.FC = () => {
  const { courseId, lessonId } = useParams<{ courseId: string; lessonId: string }>();
  const navigate = useNavigate();
  const [lesson, setLesson] = useState<Lesson | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isCompleting, setIsCompleting] = useState(false);
  const [progress, setProgress] = useState(0);
  const [startTime] = useState(Date.now());

  useEffect(() => {
    loadLesson();
  }, [lessonId]);

  // Progress tracking
  useEffect(() => {
    if (!lesson || lesson.isCompleted) return;

    const interval = setInterval(() => {
      const elapsed = Math.floor((Date.now() - startTime) / 1000);
      const progressPercent = Math.min((elapsed / (lesson.estimatedDuration * 60)) * 100, 100);
      setProgress(progressPercent);

      // Auto-save progress every 30 seconds
      if (elapsed % 30 === 0 && lessonId) {
        lessonService.updateProgress(parseInt(lessonId), {
          timeSpent: elapsed,
          isCompleted: false,
        }).catch(err => console.error('Error updating progress:', err));
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [lesson, startTime, lessonId]);

  useEffect(() => {
    const handleKeyPress = (e: KeyboardEvent) => {
      // Ne működjön, ha input mezőben vagyunk
      if (e.target instanceof HTMLInputElement || e.target instanceof HTMLTextAreaElement) {
        return;
      }

      switch (e.key) {
        case 'ArrowLeft':
          if (lesson?.previousLessonId) handlePrev();
          break;
        case 'ArrowRight':
          if (lesson?.nextLessonId) handleNext();
          break;
        case ' ':
          e.preventDefault();
          if (!lesson?.isCompleted) handleComplete();
          break;
        case 'Escape':
          navigate(`/courses/${courseId}`);
          break;
      }
    };

    window.addEventListener('keydown', handleKeyPress);
    return () => window.removeEventListener('keydown', handleKeyPress);
  }, [lesson, courseId, navigate]);

  const loadLesson = async () => {
    if (lessonId) {
      try {
        setIsLoading(true);
        const data = await lessonService.getLessonById(parseInt(lessonId));
        setLesson(data);
      } catch (error) {
        console.error('Error loading lesson:', error);
        toast.error('Hiba a lecke betöltése közben');
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleComplete = async () => {
    if (!lessonId) return;

    try {
      setIsCompleting(true);
      await lessonService.markAsCompleted(parseInt(lessonId));
      
      if (lesson) {
        setLesson({ ...lesson, isCompleted: true });
      }
      
      toast.success('Lecke teljesítve! 🎉');
      
      // Ha van következő lecke, navigáljunk oda
      if (lesson?.nextLessonId) {
        setTimeout(() => {
          navigate(`/courses/${courseId}/lessons/${lesson.nextLessonId}`);
        }, 1500);
      }
    } catch (error) {
      console.error('Error completing lesson:', error);
      toast.error('Hiba a lecke teljesítése közben');
    } finally {
      setIsCompleting(false);
    }
  };

  const handleNext = () => {
    if (lesson?.nextLessonId) {
      navigate(`/courses/${courseId}/lessons/${lesson.nextLessonId}`);
    }
  };

  const handlePrev = () => {
    if (lesson?.previousLessonId) {
      navigate(`/courses/${courseId}/lessons/${lesson.previousLessonId}`);
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!lesson) {
    return (
      <Container>
        <Alert severity="error">Lecke nem található</Alert>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(`/courses/${courseId}`)}
          sx={{ mt: 2 }}
        >
          Vissza a kurzushoz
        </Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 3 }}>
        <Link
          underline="hover"
          sx={{ display: 'flex', alignItems: 'center', cursor: 'pointer' }}
          color="inherit"
          onClick={() => navigate('/dashboard')}
        >
          <HomeIcon sx={{ mr: 0.5 }} fontSize="inherit" />
          Kezdőlap
        </Link>
        <Link
          underline="hover"
          sx={{ display: 'flex', alignItems: 'center', cursor: 'pointer' }}
          color="inherit"
          onClick={() => navigate('/my-courses')}
        >
          <SchoolIcon sx={{ mr: 0.5 }} fontSize="inherit" />
          Kurzusaim
        </Link>
        <Link
          underline="hover"
          sx={{ display: 'flex', alignItems: 'center', cursor: 'pointer' }}
          color="inherit"
          onClick={() => navigate(`/courses/${courseId}`)}
        >
          Kurzus
        </Link>
        {lesson.moduleTitle && (
          <Typography color="text.primary">{lesson.moduleTitle}</Typography>
        )}
        <Typography color="text.primary">{lesson.title}</Typography>
      </Breadcrumbs>

      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 4 }}>
        <IconButton onClick={() => navigate(`/courses/${courseId}`)}>
          <ArrowBackIcon />
        </IconButton>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>
          {lesson.title}
        </Typography>
        <Chip
          label={lesson.isCompleted ? 'Teljesítve' : 'Folyamatban'}
          color={lesson.isCompleted ? 'success' : 'primary'}
          icon={lesson.isCompleted ? <CheckCircleIcon /> : <PlayCircleIcon />}
        />
      </Box>

      {/* Progress Bar */}
      {!lesson.isCompleted && (
        <Box sx={{ mb: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
            <Typography variant="body2" color="text.secondary">
              Haladás
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {Math.round(progress)}%
            </Typography>
          </Box>
          <LinearProgress variant="determinate" value={progress} />
        </Box>
      )}

      <Grid container spacing={4}>
        {/* Main Content */}
        <Grid item xs={12} lg={8}>
          <Card>
            <CardContent>
              {lesson.type === 'Video' && lesson.videoUrl && (
                <Box sx={{ mb: 3, position: 'relative', paddingTop: '56.25%' }}>
                  <iframe
                    src={lesson.videoUrl}
                    title={lesson.title}
                    style={{
                      position: 'absolute',
                      top: 0,
                      left: 0,
                      width: '100%',
                      height: '100%',
                      border: 'none',
                    }}
                    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                    allowFullScreen
                  />
                </Box>
              )}

              <Box sx={{ p: 3 }}>
                {lesson.content ? (
                  <div dangerouslySetInnerHTML={{ __html: lesson.content }} />
                ) : (
                  <Typography color="text.secondary">
                    A lecke tartalma nem elérhető.
                  </Typography>
                )}
              </Box>

              {lesson.attachmentUrl && (
                <Box sx={{ p: 3, pt: 0 }}>
                  <Button
                    variant="outlined"
                    startIcon={<Description />}
                    href={lesson.attachmentUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    Melléklet letöltése
                  </Button>
                </Box>
              )}
            </CardContent>
          </Card>

          {/* Navigation */}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 3 }}>
            <Button
              startIcon={<ArrowBackIcon />}
              onClick={handlePrev}
              disabled={!lesson.previousLessonId}
              variant="outlined"
            >
              Előző lecke
            </Button>
            
            {lesson.isCompleted ? (
              <Button
                variant="contained"
                endIcon={<ArrowForwardIcon />}
                onClick={handleNext}
                disabled={!lesson.nextLessonId}
              >
                {lesson.nextLessonId ? 'Következő lecke' : 'Vissza a kurzushoz'}
              </Button>
            ) : (
              <Button
                variant="contained"
                endIcon={<CheckCircleIcon />}
                onClick={handleComplete}
                disabled={isCompleting}
                startIcon={isCompleting ? <CircularProgress size={20} /> : undefined}
              >
                {isCompleting ? 'Teljesítés...' : 'Lecke teljesítése'}
              </Button>
            )}
          </Box>
        </Grid>

        {/* Sidebar */}
        <Grid item xs={12} lg={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Lecke információk
              </Typography>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                <DescriptionIcon />
                <Typography variant="body2">
                  Típus: {lesson.type}
                </Typography>
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                <PlayCircleIcon />
                <Typography variant="body2">
                  Becsült idő: {lesson.estimatedDuration} perc
                </Typography>
              </Box>
              {lesson.timeSpent !== undefined && lesson.timeSpent > 0 && (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                  <CheckCircleIcon />
                  <Typography variant="body2">
                    Eltöltött idő: {Math.floor(lesson.timeSpent / 60)} perc
                  </Typography>
                </Box>
              )}
            </CardContent>
          </Card>

          {/* Module Progress */}
          <Card sx={{ mt: 3 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                {lesson.moduleTitle || 'Modul'} haladás
              </Typography>
              <Typography variant="body2" color="text.secondary" paragraph>
                Folytasd a tanulást a modul többi leckéjével
              </Typography>
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button
                  variant="outlined"
                  fullWidth
                  disabled={!lesson.previousLessonId}
                  onClick={handlePrev}
                  startIcon={<ArrowBackIcon />}
                >
                  Előző
                </Button>
                <Button
                  variant="outlined"
                  fullWidth
                  disabled={!lesson.nextLessonId}
                  onClick={handleNext}
                  endIcon={<ArrowForwardIcon />}
                >
                  Következő
                </Button>
              </Box>
            </CardContent>
          </Card>

          {/* Quiz Card */}
          <Card sx={{ mt: 3 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Kapcsolódó kvíz
              </Typography>
              <Typography variant="body2" color="text.secondary" paragraph>
                Teszteld a leckéből szerzett ismereteidet!
              </Typography>
              <Button
                variant="outlined"
                fullWidth
                onClick={() => navigate(`/quizzes/1`)}
              >
                Kvíz indítása
              </Button>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
};

export default LessonView;