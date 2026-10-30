import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchMyCourses } from '../features/course/courseSlice';
import {
  Grid,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  LinearProgress,
  Chip,
  Avatar,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Divider,
} from '@mui/material';
import {
  School as SchoolIcon,
  Quiz as QuizIcon,
  TrendingUp as TrendingUpIcon,
  AccessTime as AccessTimeIcon,
  CheckCircle as CheckCircleIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
//import { formatDistanceToNow } from 'date-fns';
//import { hu } from 'date-fns/locale';

const Dashboard: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { user } = useAppSelector((state) => state.auth);
  const { myCourses } = useAppSelector((state) => state.courses);

  useEffect(() => {
    dispatch(fetchMyCourses());
  }, [dispatch]);

  const stats = {
    totalCourses: myCourses.length,
    completedCourses: myCourses.filter(c => c.userRating).length,
    inProgressCourses: myCourses.length - myCourses.filter(c => c.userRating).length,
    averageProgress: 65, // Ez majd a backendről jön
  };

  const recentActivities = [
    { id: 1, type: 'lesson', title: 'React alapok', course: 'Webfejlesztés', time: '2 órája' },
    { id: 2, type: 'quiz', title: 'JavaScript kvíz', course: 'Webfejlesztés', score: 85, time: '1 napja' },
    { id: 3, type: 'forum', title: 'Kérdés a props-ról', course: 'Webfejlesztés', time: '2 napja' },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Üdvözöljük, {user?.firstName}!
      </Typography>
      <Typography variant="subtitle1" color="text.secondary" gutterBottom>
        Itt láthatja a tanulási folyamatát
      </Typography>

      {/* Statisztikák */}
      <Grid container spacing={3} sx={{ mt: 2, mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Avatar sx={{ bgcolor: 'primary.main', mr: 2 }}>
                  <SchoolIcon />
                </Avatar>
                <Typography variant="h6">Kurzusok</Typography>
              </Box>
              <Typography variant="h4">{stats.totalCourses}</Typography>
              <Typography variant="body2" color="text.secondary">
                Összes kurzus
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Avatar sx={{ bgcolor: 'success.main', mr: 2 }}>
                  <CheckCircleIcon />
                </Avatar>
                <Typography variant="h6">Teljesített</Typography>
              </Box>
              <Typography variant="h4">{stats.completedCourses}</Typography>
              <Typography variant="body2" color="text.secondary">
                Befejezett kurzus
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Avatar sx={{ bgcolor: 'warning.main', mr: 2 }}>
                  <AccessTimeIcon />
                </Avatar>
                <Typography variant="h6">Folyamatban</Typography>
              </Box>
              <Typography variant="h4">{stats.inProgressCourses}</Typography>
              <Typography variant="body2" color="text.secondary">
                Aktív kurzus
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Avatar sx={{ bgcolor: 'info.main', mr: 2 }}>
                  <TrendingUpIcon />
                </Avatar>
                <Typography variant="h6">Átlagos haladás</Typography>
              </Box>
              <Typography variant="h4">{stats.averageProgress}%</Typography>
              <Box sx={{ mt: 1 }}>
                <LinearProgress 
                  variant="determinate" 
                  value={stats.averageProgress} 
                  sx={{ height: 8, borderRadius: 4 }}
                />
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Aktív kurzusok */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Aktív kurzusaim
            </Typography>
            <List>
              {myCourses.slice(0, 3).map((course) => (
                <React.Fragment key={course.id}>
                  <ListItem
                    button
                    onClick={() => navigate(`/courses/${course.id}`)}
                  >
                    <ListItemAvatar>
                      <Avatar
                        src={course.thumbnailUrl}
                        sx={{ bgcolor: 'primary.main' }}
                      >
                        <SchoolIcon />
                      </Avatar>
                    </ListItemAvatar>
                    <ListItemText
                      primary={course.title}
                      secondary={
                        <>
                          <Typography component="span" variant="body2" color="text.primary">
                            {course.teacherName}
                          </Typography>
                          {` — ${course.difficulty}`}
                        </>
                      }
                    />
                    <Chip
                      label={course.isEnrolled ? 'Feliratkozva' : 'Ingyenes'}
                      color={course.isEnrolled ? 'success' : 'primary'}
                      size="small"
                    />
                  </ListItem>
                  <Divider variant="inset" component="li" />
                </React.Fragment>
              ))}
            </List>
            <Box sx={{ textAlign: 'center', mt: 2 }}>
              <Button onClick={() => navigate('/my-courses')}>
                Összes kurzus megtekintése
              </Button>
            </Box>
          </Paper>
        </Grid>

        {/* Legutóbbi tevékenységek */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Legutóbbi tevékenységek
            </Typography>
            <List>
              {recentActivities.map((activity) => (
                <ListItem key={activity.id} sx={{ py: 1 }}>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'primary.light' }}>
                      {activity.type === 'quiz' ? <QuizIcon /> : <SchoolIcon />}
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                    primary={activity.title}
                    secondary={
                      <>
                        <Typography component="span" variant="body2" color="text.primary">
                          {activity.course}
                        </Typography>
                        {activity.score && ` — ${activity.score}%`}
                        <br />
                        {activity.time}
                      </>
                    }
                  />
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default Dashboard;