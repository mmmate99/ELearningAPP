import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { fetchMyCourses } from '../../features/course/courseSlice';
import {
  Grid,
  Card,
  CardContent,
  CardMedia,
  CardActions,
  Button,
  Typography,
  Box,
  Chip,
  LinearProgress,
  CircularProgress,
} from '@mui/material';
import {
  PlayCircle as PlayCircleIcon,
  School as SchoolIcon,
  CheckCircle as CheckCircleIcon,
} from '@mui/icons-material';

const MyCourses: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { myCourses, isLoading } = useAppSelector((state) => state.courses);

  useEffect(() => {
    dispatch(fetchMyCourses());
  }, [dispatch]);

  /*const getProgress = (course: any) => {
    // Ez majd a backendről jön, most mock
    return Math.floor(Math.random() * 100);
  };*/

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Kurzusaim
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom sx={{ mb: 4 }}>
        {myCourses.length} aktív kurzus
      </Typography>

      {myCourses.length === 0 ? (
        <Box textAlign="center" py={8}>
          <SchoolIcon sx={{ fontSize: 80, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" gutterBottom>
            Még nincs feliratkozva kurzusokra
          </Typography>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            Fedezd fel a kurzusokat és kezdd el a tanulást!
          </Typography>
          <Button
            variant="contained"
            sx={{ mt: 2 }}
            onClick={() => navigate('/courses')}
          >
            Kurzusok böngészése
          </Button>
        </Box>
      ) : (
        <Grid container spacing={3}>
          {myCourses.map((course) => {
            //const progress = getProgress(course);
            return (
              <Grid item xs={12} md={6} lg={4} key={course.id}>
                <Card
                  sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    cursor: 'pointer',
                    transition: 'transform 0.2s',
                    '&:hover': {
                      transform: 'translateY(-4px)',
                    },
                  }}
                  onClick={() => navigate(`/courses/${course.id}`)}
                >
                  <CardMedia
                    component="img"
                    height="140"
                    image={course.thumbnailUrl || '/default-course.jpg'}
                    alt={course.title}
                    sx={{ objectFit: 'cover' }}
                  />
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                      <Chip
                        label={course.difficulty}
                        size="small"
                        color={
                          course.difficulty === 'Beginner'
                            ? 'success'
                            : course.difficulty === 'Intermediate'
                            ? 'warning'
                            : 'error'
                        }
                      />
                      {course.userRating && (
                        <Chip
                          icon={<CheckCircleIcon />}
                          label="Teljesítve"
                          size="small"
                          color="success"
                          variant="outlined"
                        />
                      )}
                    </Box>
                    <Typography variant="h6" gutterBottom noWrap>
                      {course.title}
                    </Typography>
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                      {course.teacherName}
                    </Typography>
                    
                    {/* Progress */}
                    <Box sx={{ mt: 2 }}>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
                        <Typography variant="body2" color="text.secondary">
                          Haladás
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          {/*progress*/}%
                        </Typography>
                      </Box>
                      <LinearProgress
                        variant="determinate"
                        //value={progress}
                        sx={{ height: 8, borderRadius: 4 }}
                      />
                    </Box>
                  </CardContent>
                  <CardActions>
                    <Button
                      size="small"
                      startIcon={<PlayCircleIcon />}
                      onClick={(e) => {
                        e.stopPropagation();
                        navigate(`/courses/${course.id}`);
                      }}
                      fullWidth
                      variant="contained"
                    >
                      {course.userRating ? 'Megtekintés' : 'Folytatás'}
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      )}
    </Box>
  );
};

export default MyCourses;