import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { fetchTeachingCourses } from '../../features/course/courseSlice';
import {
  Grid,
  Card,
  CardContent,
  CardActions,
  Button,
  Typography,
  Box,
  Chip,
  IconButton,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  CircularProgress,
} from '@mui/material';
import {
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  Add as AddIcon,
  People as PeopleIcon,
  School as SchoolIcon,
} from '@mui/icons-material';
import { CreateCourseRequest, DifficultyLevel } from '../../types/course';

const TeachingCourses: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { teachingCourses, isLoading } = useAppSelector((state) => state.courses);
  const { user } = useAppSelector((state) => state.auth);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedCourse, setSelectedCourse] = useState<number | null>(null);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [courseData, setCourseData] = useState<CreateCourseRequest>({
    title: '',
    description: '',
    price: 0,
    isFree: true,
    difficulty: DifficultyLevel.Beginner,
  });

  useEffect(() => {
    if (user?.role === 'Teacher' || user?.role === 'Admin') {
      dispatch(fetchTeachingCourses());
    }
  }, [dispatch, user]);

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>, courseId: number) => {
    setAnchorEl(event.currentTarget);
    setSelectedCourse(courseId);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedCourse(null);
  };

  const handleEdit = () => {
    if (selectedCourse) {
      navigate(`/courses/${selectedCourse}/edit`);
      handleMenuClose();
    }
  };

  const handleDelete = () => {
    // Implement delete logic
    console.log('Delete course', selectedCourse);
    handleMenuClose();
  };

  const handleCreateCourse = async () => {
    // Implement create logic
    console.log('Create course', courseData);
    setCreateDialogOpen(false);
    setCourseData({
      title: '',
      description: '',
      price: 0,
      isFree: true,
      difficulty: DifficultyLevel.Beginner,
    });
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (user?.role !== 'Teacher' && user?.role !== 'Admin') {
    return (
      <Box textAlign="center" py={8}>
        <Typography variant="h6" color="error" gutterBottom>
          Nincs jogosultságod a tanított kurzusok megtekintéséhez
        </Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            Tanított kurzusok
          </Typography>
          <Typography variant="body1" color="text.secondary">
            {teachingCourses.length} kurzus
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => setCreateDialogOpen(true)}
        >
          Új kurzus
        </Button>
      </Box>

      {teachingCourses.length === 0 ? (
        <Box textAlign="center" py={8}>
          <SchoolIcon sx={{ fontSize: 80, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" gutterBottom>
            Még nincs létrehozott kurzusod
          </Typography>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            Hozz létre első kurzusodat!
          </Typography>
        </Box>
      ) : (
        <Grid container spacing={3}>
          {teachingCourses.map((course) => (
            <Grid item xs={12} md={6} lg={4} key={course.id}>
              <Card
                sx={{
                  height: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  position: 'relative',
                }}
              >
                <Box sx={{ position: 'absolute', top: 8, right: 8 }}>
                  <IconButton
                    size="small"
                    onClick={(e) => handleMenuClick(e, course.id)}
                  >
                    <MoreVertIcon />
                  </IconButton>
                </Box>
                <CardContent sx={{ flexGrow: 1, pt: 6 }}>
                  <Typography variant="h6" gutterBottom>
                    {course.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary" paragraph>
                    {course.description.length > 150
                      ? `${course.description.substring(0, 150)}...`
                      : course.description}
                  </Typography>
                  <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 2 }}>
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
                    <Chip
                      icon={<PeopleIcon />}
                      label={`${course.studentCount} diák`}
                      size="small"
                      variant="outlined"
                    />
                    <Chip
                      label={course.isPublished ? 'Publikált' : 'Piszkozat'}
                      size="small"
                      color={course.isPublished ? 'success' : 'default'}
                    />
                  </Box>
                </CardContent>
                <CardActions>
                  <Button
                    size="small"
                    startIcon={<VisibilityIcon />}
                    onClick={() => navigate(`/courses/${course.id}`)}
                    fullWidth
                  >
                    Megtekintés
                  </Button>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {/* Course Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
      >
        <MenuItem onClick={handleEdit}>
          <EditIcon sx={{ mr: 2 }} />
          Szerkesztés
        </MenuItem>
        <MenuItem onClick={handleDelete}>
          <DeleteIcon sx={{ mr: 2 }} />
          Törlés
        </MenuItem>
      </Menu>

      {/* Create Course Dialog */}
      <Dialog open={createDialogOpen} onClose={() => setCreateDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Új kurzus létrehozása</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <TextField
              label="Kurzus címe"
              value={courseData.title}
              onChange={(e) => setCourseData({ ...courseData, title: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Leírás"
              value={courseData.description}
              onChange={(e) => setCourseData({ ...courseData, description: e.target.value })}
              multiline
              rows={4}
              fullWidth
              required
            />
            <Box sx={{ display: 'flex', gap: 2 }}>
              <TextField
                label="Ár"
                type="number"
                value={courseData.price}
                onChange={(e) => setCourseData({ ...courseData, price: parseFloat(e.target.value) })}
                fullWidth
                disabled={courseData.isFree}
              />
              <Button
                variant={courseData.isFree ? 'contained' : 'outlined'}
                onClick={() => setCourseData({ ...courseData, isFree: !courseData.isFree })}
              >
                {courseData.isFree ? 'Ingyenes' : 'Fizetős'}
              </Button>
            </Box>
            <TextField
              select
              label="Nehézségi szint"
              value={courseData.difficulty}
              onChange={(e) => setCourseData({ ...courseData, difficulty: e.target.value as DifficultyLevel })}
              fullWidth
              SelectProps={{
                native: true,
              }}
            >
              {Object.values(DifficultyLevel).map((level) => (
                <option key={level} value={level}>
                  {level}
                </option>
              ))}
            </TextField>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateDialogOpen(false)}>Mégse</Button>
          <Button onClick={handleCreateCourse} variant="contained" disabled={!courseData.title || !courseData.description}>
            Létrehozás
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default TeachingCourses;