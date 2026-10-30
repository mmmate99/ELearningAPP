import React, { useState } from 'react';
import { useAppSelector, useAppDispatch } from '../store/hooks';
import { setUser } from '../features/auth/authSlice';
import {
  Container,
  Paper,
  Typography,
  Box,
  TextField,
  Button,
  Avatar,
  Grid,
  Card,
  Chip,
  CardContent,
  Divider,
  Alert,
  CircularProgress,
  IconButton,
} from '@mui/material';
import {
  Edit as EditIcon,
  Save as SaveIcon,
  Cancel as CancelIcon,
  PhotoCamera as PhotoCameraIcon,
  School as SchoolIcon,
  Quiz as QuizIcon,
  Forum as ForumIcon,
} from '@mui/icons-material';
//import { authService } from '../services/authService';

const Profile: React.FC = () => {
  const dispatch = useAppDispatch();
  const { user } = useAppSelector((state) => state.auth);
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    email: user?.email || '',
    bio: user?.bio || '',
  });

  const stats = {
    courses: 5,
    quizzes: 12,
    forumPosts: 8,
    completed: 3,
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSave = async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      // Update profile logic here
      const updatedUser = {
        ...user,
        ...formData,
      };
      
      // This would be an API call in real app
      dispatch(setUser(updatedUser));
      
      setSuccess('Profil sikeresen frissítve!');
      setIsEditing(false);
    } catch (error: any) {
      setError(error.message || 'Hiba történt a profil frissítése során');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    setFormData({
      firstName: user?.firstName || '',
      lastName: user?.lastName || '',
      email: user?.email || '',
      bio: user?.bio || '',
    });
    setIsEditing(false);
    setError(null);
    setSuccess(null);
  };

  if (!user) {
    return (
      <Container>
        <Alert severity="error">Felhasználói adatok betöltése sikertelen</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Grid container spacing={4}>
        {/* Left Column - Profile */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3, textAlign: 'center' }}>
            <Box sx={{ position: 'relative', display: 'inline-block', mb: 2 }}>
              <Avatar
                src={user.profileImageUrl}
                sx={{ width: 120, height: 120, mx: 'auto', mb: 2 }}
              >
                {user.firstName.charAt(0)}{user.lastName.charAt(0)}
              </Avatar>
              {isEditing && (
                <IconButton
                  sx={{
                    position: 'absolute',
                    bottom: 10,
                    right: 10,
                    bgcolor: 'background.paper',
                  }}
                >
                  <PhotoCameraIcon />
                </IconButton>
              )}
            </Box>
            
            {isEditing ? (
              <Box component="form" sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Keresztnév"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleChange}
                  margin="normal"
                />
                <TextField
                  fullWidth
                  label="Vezetéknév"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleChange}
                  margin="normal"
                />
                <TextField
                  fullWidth
                  label="Email"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  margin="normal"
                  type="email"
                />
                <TextField
                  fullWidth
                  label="Bio"
                  name="bio"
                  value={formData.bio}
                  onChange={handleChange}
                  margin="normal"
                  multiline
                  rows={3}
                />
              </Box>
            ) : (
              <>
                <Typography variant="h5" gutterBottom>
                  {user.fullName}
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {user.email}
                </Typography>
                <Typography variant="body1" sx={{ mt: 2 }}>
                  {user.bio || 'Nincs bio megadva.'}
                </Typography>
                <Chip
                  label={user.role}
                  color="primary"
                  sx={{ mt: 2 }}
                />
              </>
            )}

            <Box sx={{ mt: 3 }}>
              {isEditing ? (
                <Box sx={{ display: 'flex', gap: 1 }}>
                  <Button
                    variant="contained"
                    startIcon={isLoading ? <CircularProgress size={20} /> : <SaveIcon />}
                    onClick={handleSave}
                    disabled={isLoading}
                    fullWidth
                  >
                    Mentés
                  </Button>
                  <Button
                    variant="outlined"
                    startIcon={<CancelIcon />}
                    onClick={handleCancel}
                    fullWidth
                  >
                    Mégse
                  </Button>
                </Box>
              ) : (
                <Button
                  variant="outlined"
                  startIcon={<EditIcon />}
                  onClick={() => setIsEditing(true)}
                  fullWidth
                >
                  Szerkesztés
                </Button>
              )}
            </Box>

            {error && (
              <Alert severity="error" sx={{ mt: 2 }}>
                {error}
              </Alert>
            )}
            {success && (
              <Alert severity="success" sx={{ mt: 2 }}>
                {success}
              </Alert>
            )}
          </Paper>
        </Grid>

        {/* Right Column - Stats and Activity */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              Statisztikák
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={6} sm={3}>
                <Card>
                  <CardContent sx={{ textAlign: 'center' }}>
                    <SchoolIcon color="primary" sx={{ fontSize: 40, mb: 1 }} />
                    <Typography variant="h5">{stats.courses}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      Kurzus
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Card>
                  <CardContent sx={{ textAlign: 'center' }}>
                    <QuizIcon color="secondary" sx={{ fontSize: 40, mb: 1 }} />
                    <Typography variant="h5">{stats.quizzes}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      Kvíz
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Card>
                  <CardContent sx={{ textAlign: 'center' }}>
                    <ForumIcon color="success" sx={{ fontSize: 40, mb: 1 }} />
                    <Typography variant="h5">{stats.forumPosts}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      Fórum poszt
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Card>
                  <CardContent sx={{ textAlign: 'center' }}>
                    <SchoolIcon color="warning" sx={{ fontSize: 40, mb: 1 }} />
                    <Typography variant="h5">{stats.completed}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      Teljesített
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            </Grid>
          </Paper>

          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Legutóbbi tevékenységek
            </Typography>
            <Divider sx={{ mb: 2 }} />
            <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
              Még nincs tevékenység
            </Typography>
            {/* Activity list would go here */}
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
};

export default Profile;