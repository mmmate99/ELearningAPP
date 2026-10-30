import React, { useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { fetchCourses, enrollInCourse } from '../../features/course/courseSlice';
import {
  Grid,
  Card,
  CardContent,
  CardMedia,
  CardActions,
  Button,
  Typography,
  Box,
  TextField,
  InputAdornment,
  Chip,
  Rating,
  CircularProgress,
  Pagination,
} from '@mui/material';
import {
  Search as SearchIcon,
  School as SchoolIcon,
  People as PeopleIcon,
  PlayCircle as PlayCircleIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

const CourseList: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { courses, isLoading } = useAppSelector((state) => state.courses);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(1);
  const itemsPerPage = 9;

  useEffect(() => {
    dispatch(fetchCourses());
  }, [dispatch]);

  const handleEnroll = async (courseId: number, e: React.MouseEvent) => {
    e.stopPropagation();
    await dispatch(enrollInCourse(courseId));
  };

  const filteredCourses = courses.filter(course =>
    course.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    course.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const paginatedCourses = filteredCourses.slice(
    (page - 1) * itemsPerPage,
    page * itemsPerPage
  );

  const getDifficultyColor = (difficulty: string) => {
    switch (difficulty) {
      case 'Beginner': return 'success';
      case 'Intermediate': return 'warning';
      case 'Advanced': return 'error';
      default: return 'default';
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom>
          Kurzusok
        </Typography>
        <Typography variant="body1" color="text.secondary" gutterBottom>
          Fedezd fel sokszínű kurzusainkat és bővítsd tudásodat
        </Typography>
      </Box>

      <Box sx={{ mb: 4 }}>
        <TextField
          fullWidth
          variant="outlined"
          placeholder="Keresés kurzusok között..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          sx={{ maxWidth: 600 }}
        />
      </Box>

      <Grid container spacing={3}>
        {paginatedCourses.map((course) => (
          <Grid item xs={12} sm={6} md={4} key={course.id}>
            <Card 
              sx={{ 
                height: '100%', 
                display: 'flex', 
                flexDirection: 'column',
                cursor: 'pointer',
                transition: 'transform 0.2s',
                '&:hover': {
                  transform: 'translateY(-4px)',
                  boxShadow: 6,
                }
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
                    color={getDifficultyColor(course.difficulty) as any}
                    size="small"
                  />
                  <Typography variant="body2" color="text.secondary">
                    {course.isFree ? 'Ingyenes' : `${course.price} Ft`}
                  </Typography>
                </Box>
                <Typography variant="h6" gutterBottom noWrap>
                  {course.title}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  {course.description.length > 100
                    ? `${course.description.substring(0, 100)}...`
                    : course.description}
                </Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                  <SchoolIcon fontSize="small" color="action" />
                  <Typography variant="body2" color="text.secondary">
                    {course.teacherName}
                  </Typography>
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                  <PeopleIcon fontSize="small" color="action" />
                  <Typography variant="body2" color="text.secondary">
                    {course.studentCount} diák
                  </Typography>
                </Box>
                {course.averageRating && (
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Rating
                      value={course.averageRating}
                      readOnly
                      size="small"
                      precision={0.5}
                    />
                    <Typography variant="body2" color="text.secondary">
                      ({course.averageRating.toFixed(1)})
                    </Typography>
                  </Box>
                )}
              </CardContent>
              <CardActions>
                <Button
                  size="small"
                  startIcon={course.isEnrolled ? <PlayCircleIcon /> : <SchoolIcon />}
                  onClick={(e) => handleEnroll(course.id, e)}
                  disabled={course.isEnrolled}
                  fullWidth
                  variant={course.isEnrolled ? "outlined" : "contained"}
                >
                  {course.isEnrolled ? 'Folytatás' : 'Feliratkozás'}
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>

      {filteredCourses.length > itemsPerPage && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <Pagination
            count={Math.ceil(filteredCourses.length / itemsPerPage)}
            page={page}
            onChange={(_, value) => setPage(value)}
            color="primary"
          />
        </Box>
      )}
    </Box>
  );
};

export default CourseList;