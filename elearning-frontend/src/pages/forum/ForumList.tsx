import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
//import { useAppSelector } from '../../store/hooks';
import {
  Grid,
  Card,
  CardContent,
  CardActions,
  Button,
  Typography,
  Box,
  Chip,
  TextField,
  InputAdornment,
  Avatar,
  CircularProgress,
} from '@mui/material';
import {
  Search as SearchIcon,
  Add as AddIcon,
  Forum as ForumIcon,
  PushPin as PushPinIcon,
  Comment as CommentIcon,
} from '@mui/icons-material';
//import { forumService } from '../../services/forumService';
import { ForumPost } from '../../types/forum';

const ForumList: React.FC = () => {
  const navigate = useNavigate();
  //const { user } = useAppSelector((state) => state.auth);
  const [posts, setPosts] = useState<ForumPost[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadPosts();
  }, []);

  const loadPosts = async () => {
    try {
      setIsLoading(true);
      // This would come from backend - for now mock data
      const mockPosts: ForumPost[] = [
        {
          id: 1,
          title: 'Hogyan kezdjem a React tanulását?',
          content: 'Sziasztok! Új vagyok a React világában. Ti hogyan kezdtétek?',
          createdAt: '2024-01-10T10:30:00Z',
          isPinned: true,
          author: {
            id: '1',
            email: 'student@example.com',
            firstName: 'János',
            lastName: 'Kovács',
            fullName: 'János Kovács',
            role: 'Student',
            createdAt: '2024-01-01',
          },
          courseId: 1,
          replies: [],
        },
        {
          id: 2,
          title: 'TypeScript vs JavaScript',
          content: 'Melyiket érdemes tanulni először? Melyik a jobb karrier szempontjából?',
          createdAt: '2024-01-09T14:20:00Z',
          isPinned: false,
          author: {
            id: '2',
            email: 'teacher@example.com',
            firstName: 'Anna',
            lastName: 'Nagy',
            fullName: 'Anna Nagy',
            role: 'Teacher',
            createdAt: '2024-01-01',
          },
          courseId: 1,
          replies: [],
        },
        {
          id: 3,
          title: 'Projekt ötletek kezdőknek',
          content: 'Szeretnék gyakorolni, de nem tudom milyen projekteket csináljak. Ti milyen projekteket ajánlotok?',
          createdAt: '2024-01-08T09:15:00Z',
          isPinned: false,
          author: {
            id: '3',
            email: 'student2@example.com',
            firstName: 'Péter',
            lastName: 'Tóth',
            fullName: 'Péter Tóth',
            role: 'Student',
            createdAt: '2024-01-01',
          },
          courseId: 2,
          replies: [],
        },
      ];
      setPosts(mockPosts);
    } catch (error) {
      console.error('Error loading forum posts:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredPosts = posts.filter(post =>
    post.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    post.content.toLowerCase().includes(searchTerm.toLowerCase()) ||
    post.author.fullName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            Fórum
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Beszélj más diákokkal és tanárokkal
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate('/forum/new')}
        >
          Új téma
        </Button>
      </Box>

      <Box sx={{ mb: 4 }}>
        <TextField
          fullWidth
          variant="outlined"
          placeholder="Keresés a fórumban..."
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
        {filteredPosts.map((post) => (
          <Grid item xs={12} key={post.id}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Avatar src={post.author.profileImageUrl}>
                      {post.author.firstName.charAt(0)}
                    </Avatar>
                    <Box>
                      <Typography variant="h6">
                        {post.title}
                        {post.isPinned && (
                          <PushPinIcon fontSize="small" sx={{ ml: 1, verticalAlign: 'middle', color: 'warning.main' }} />
                        )}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {post.author.fullName} • {new Date(post.createdAt).toLocaleDateString()}
                      </Typography>
                    </Box>
                  </Box>
                  <Chip
                    label={`${post.replies.length} válasz`}
                    icon={<CommentIcon />}
                    size="small"
                    variant="outlined"
                  />
                </Box>
                <Typography variant="body2" color="text.secondary" paragraph>
                  {post.content.length > 300
                    ? `${post.content.substring(0, 300)}...`
                    : post.content}
                </Typography>
                <Box sx={{ display: 'flex', gap: 1 }}>
                  <Chip label={`Kurzus #${post.courseId}`} size="small" />
                </Box>
              </CardContent>
              <CardActions>
                <Button
                  size="small"
                  onClick={() => navigate(`/forum/${post.id}`)}
                >
                  Téma megtekintése
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>

      {filteredPosts.length === 0 && (
        <Box textAlign="center" py={8}>
          <ForumIcon sx={{ fontSize: 80, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" gutterBottom>
            Nincs találat
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Próbálj meg másik keresési kifejezést, vagy indíts új témát!
          </Typography>
        </Box>
      )}
    </Box>
  );
};

export default ForumList;