import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { fetchCourseDetails, enrollInCourse } from '../../features/course/courseSlice';
import CourseProgress from '../../components/course/CourseProgress';
import {
  Box,
  Typography,
  Button,
  Chip,
  Rating,
  Card,
  CardContent,
  Grid,
  Tab,
  Tabs,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  PlayCircle as PlayCircleIcon,
  Book as BookIcon,
  Quiz as QuizIcon,
  Forum as ForumIcon,
  People as PeopleIcon,
  AccessTime as AccessTimeIcon,
  School as SchoolIcon,
  Star as StarIcon,
  CheckCircle as CheckCircleIcon,
} from '@mui/icons-material';
import { Lesson, Module } from '../../types/course';

// Temporary remove ForumPost import - we'll fix types later

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({ children, value, index, ...other }) => {
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`course-tabpanel-${index}`}
      aria-labelledby={`course-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
};

const CourseDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { currentCourse, isLoading } = useAppSelector((state) => state.courses);
  const [tabValue, setTabValue] = useState(0);
  const [ratingDialogOpen, setRatingDialogOpen] = useState(false);
  const [rating, setRating] = useState(0);
  const [review, setReview] = useState('');

  useEffect(() => {
    if (id) {
      dispatch(fetchCourseDetails(parseInt(id)));
    }
  }, [dispatch, id]);

  const handleEnroll = async () => {
    if (id) {
      await dispatch(enrollInCourse(parseInt(id)));
    }
  };

  const handleRateCourse = async () => {
    if (id) {
      // Implement rating logic here
      console.log('Rating course:', { rating, review });
      setRatingDialogOpen(false);
      setRating(0);
      setReview('');
    }
  };

  const handleLessonClick = (lessonId: number) => {
    navigate(`/courses/${id}/lessons/${lessonId}`);
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!currentCourse) {
    return (
      <Alert severity="error" sx={{ mt: 2 }}>
        A kurzus nem található
      </Alert>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
          <Box>
            <Typography variant="h4" gutterBottom>
              {currentCourse.title}
            </Typography>
            <Typography variant="subtitle1" color="text.secondary" gutterBottom>
              {currentCourse.description}
            </Typography>
          </Box>
          <Box>
            {!currentCourse.isEnrolled ? (
              <Button
                variant="contained"
                size="large"
                startIcon={<SchoolIcon />}
                onClick={handleEnroll}
                disabled={currentCourse.isEnrolled}
              >
                {currentCourse.isFree ? 'Ingyenes feliratkozás' : `Feliratkozás - ${currentCourse.price} Ft`}
              </Button>
            ) : (
              <Button
                variant="outlined"
                startIcon={currentCourse.userRating ? <StarIcon /> : <CheckCircleIcon />}
                onClick={() => setRatingDialogOpen(true)}
              >
                {currentCourse.userRating ? 'Értékelés módosítása' : 'Kurzus értékelése'}
              </Button>
            )}
          </Box>
        </Box>

        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: 2 }}>
          <Chip
            label={currentCourse.difficulty}
            color={
              currentCourse.difficulty === 'Beginner'
                ? 'success'
                : currentCourse.difficulty === 'Intermediate'
                ? 'warning'
                : 'error'
            }
          />
          <Chip
            icon={<PeopleIcon />}
            label={`${currentCourse.studentCount} diák`}
            variant="outlined"
          />
          <Chip
            icon={<SchoolIcon />}
            label={currentCourse.teacherName}
            variant="outlined"
          />
          {currentCourse.averageRating && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Rating value={currentCourse.averageRating} readOnly size="small" />
              <Typography variant="body2" color="text.secondary">
                ({currentCourse.averageRating.toFixed(1)})
              </Typography>
            </Box>
          )}
        </Box>
      </Box>

      {/* Tabs */}
      <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tabs value={tabValue} onChange={(_, newValue) => setTabValue(newValue)}>
          <Tab label="Tartalom" icon={<BookIcon />} iconPosition="start" />
          <Tab label="Kvízek" icon={<QuizIcon />} iconPosition="start" />
          <Tab label="Fórum" icon={<ForumIcon />} iconPosition="start" />
          <Tab label="Részletek" icon={<AccessTimeIcon />} iconPosition="start" />
        </Tabs>
      </Box>

      {/* Content Tab */}
      <TabPanel value={tabValue} index={0}>
        {currentCourse.modules.map((module: Module) => (
          <Accordion key={module.id} defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              <Typography variant="h6">{module.title}</Typography>
            </AccordionSummary>
            <AccordionDetails>
              <List>
                {module.lessons.map((lesson: Lesson) => (
                  <React.Fragment key={lesson.id}>
                    <ListItem
                      sx={{
                        cursor: 'pointer',
                        borderLeft: lesson.isCompleted ? '4px solid green' : 'none',
                        '&:hover': { backgroundColor: 'action.hover' }
                      }}
                      onClick={() => handleLessonClick(lesson.id)}
                    >
                      <ListItemIcon>
                        {lesson.isCompleted ? (
                          <CheckCircleIcon color="success" />
                        ) : (
                          <PlayCircleIcon />
                        )}
                      </ListItemIcon>
                      <ListItemText
                        primary={lesson.title}
                        secondary={`${lesson.estimatedDuration} perc • ${lesson.type}`}
                      />
                      {lesson.timeSpent && (
                        <Typography variant="body2" color="text.secondary">
                          {Math.floor(lesson.timeSpent / 60)}p
                        </Typography>
                      )}
                    </ListItem>
                    <Divider variant="inset" component="li" />
                  </React.Fragment>
                ))}
              </List>
            </AccordionDetails>
          </Accordion>
        ))}
      </TabPanel>

      {/* Quizzes Tab */}
      <TabPanel value={tabValue} index={1}>
        <Grid container spacing={3}>
          {currentCourse.quizzes.map((quiz) => (
            <Grid item xs={12} md={6} key={quiz.id}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {quiz.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    {quiz.description}
                  </Typography>
                  <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mt: 2 }}>
                    <Chip label={`${quiz.passPercentage}% átmeneti`} size="small" />
                    <Chip label={`${quiz.timeLimit} perc`} size="small" />
                    <Chip label={`${quiz.maxAttempts} próbálkozás`} size="small" />
                  </Box>
                  <Button
                    variant="contained"
                    fullWidth
                    sx={{ mt: 2 }}
                    onClick={() => navigate(`/quizzes/${quiz.id}`)}
                  >
                    Kvíz kitöltése
                  </Button>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      </TabPanel>

      {/* Forum Tab - Temporarily simplified */}
      <TabPanel value={tabValue} index={2}>
        <Button
          variant="contained"
          sx={{ mb: 3 }}
          onClick={() => navigate(`/forum/new?courseId=${id}`)}
        >
          Új téma indítása
        </Button>
        <Typography>Fórum funkció hamarosan elérhető...</Typography>
      </TabPanel>

      {/* Details Tab */}
      <TabPanel value={tabValue} index={3}>
        <Grid container spacing={3}>
          {currentCourse.isEnrolled &&(
            <Grid item xs={12}>
              <Card>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    Az Ön haladása
                  </Typography>
                  <CourseProgress
                    totalLessons={currentCourse.modules.reduce((acc, module) => acc + module.lessons.length, 0)}
                    completedLessons={currentCourse.modules.reduce(
                      (acc, module) => acc + module.lessons.filter(l => l.isCompleted).length,
                      0
                    )}
                    totalDuration={currentCourse.modules.reduce(
                      (acc, module) => acc + module.lessons.reduce((sum, l) => sum + l.estimatedDuration, 0),
                      0
                    )}
                    timeSpent={currentCourse.modules.reduce(
                      (acc, module) => acc + module.lessons.reduce((sum, l) => sum + (l.timeSpent || 0), 0),
                      0
                    )}
                  />
                </CardContent>
              </Card>
            </Grid>
          )}
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Kurzus információk
                </Typography>
                <List>
                  <ListItem>
                    <ListItemText
                      primary="Tanár"
                      secondary={currentCourse.teacherName}
                    />
                  </ListItem>
                  <ListItem>
                    <ListItemText
                      primary="Nehézségi szint"
                      secondary={currentCourse.difficulty}
                    />
                  </ListItem>
                  <ListItem>
                    <ListItemText
                      primary="Diákok száma"
                      secondary={currentCourse.studentCount}
                    />
                  </ListItem>
                  <ListItem>
                    <ListItemText
                      primary="Létrehozva"
                      secondary={new Date(currentCourse.createdAt).toLocaleDateString()}
                    />
                  </ListItem>
                </List>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Statisztikák
                </Typography>
                <List>
                  <ListItem>
                    <ListItemText
                      primary="Modulok"
                      secondary={currentCourse.modules.length}
                    />
                  </ListItem>
                  <ListItem>
                    <ListItemText
                      primary="Leckék"
                      secondary={currentCourse.modules.reduce((acc, module) => acc + module.lessons.length, 0)}
                    />
                  </ListItem>
                  <ListItem>
                    <ListItemText
                      primary="Kvízek"
                      secondary={currentCourse.quizzes.length}
                    />
                  </ListItem>
                  <ListItem>
                    <ListItemText
                      primary="Fórum posztok"
                      secondary={currentCourse.forumPosts ? currentCourse.forumPosts.length : 0}
                    />
                  </ListItem>
                </List>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </TabPanel>

      {/* Rating Dialog */}
      <Dialog open={ratingDialogOpen} onClose={() => setRatingDialogOpen(false)}>
        <DialogTitle>Kurzus értékelése</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, minWidth: 400, pt: 2 }}>
            <Typography component="legend">Értékelés</Typography>
            <Rating
              value={rating}
              onChange={(_, newValue) => setRating(newValue || 0)}
              size="large"
            />
            <Typography variant="body2" color="text.secondary">
              Az értékelés funkció hamarosan elérhető lesz.
            </Typography>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRatingDialogOpen(false)}>Mégse</Button>
          <Button onClick={handleRateCourse} variant="contained" disabled>
            Hamarosan...
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default CourseDetails;