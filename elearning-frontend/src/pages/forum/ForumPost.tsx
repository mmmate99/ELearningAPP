import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppSelector } from '../../store/hooks';
import {
  Container,
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  TextField,
  Avatar,
  IconButton,
  CircularProgress,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Reply as ReplyIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  PushPin as PushPinIcon,
} from '@mui/icons-material';

interface Post {
  id: number;
  title: string;
  content: string;
  createdAt: string;
  updatedAt?: string;
  isPinned: boolean;
  author: {
    id: string;
    firstName: string;
    lastName: string;
    fullName: string;
    profileImageUrl?: string;
  };
  replies: Post[];
}

const ForumPost: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAppSelector((state) => state.auth);
  const [post, setPost] = useState<Post | null>(null);
  const [replyContent, setReplyContent] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Mock data
    const mockPost: Post = {
      id: 1,
      title: 'Hogyan kezdjem a React tanulását?',
      content: 'Sziasztok! Új vagyok a React világában. Ti hogyan kezdtétek? Milyen forrásokat ajánlotok?',
      createdAt: '2024-01-10T10:30:00Z',
      isPinned: true,
      author: {
        id: '1',
        firstName: 'János',
        lastName: 'Kovács',
        fullName: 'János Kovács',
        profileImageUrl: undefined,
      },
      replies: [
        {
          id: 2,
          title: 'Re: Hogyan kezdjem a React tanulását?',
          content: 'Én a React dokumentációval kezdtem, az nagyon jó!',
          createdAt: '2024-01-10T11:30:00Z',
          isPinned: false,
          author: {
            id: '2',
            firstName: 'Anna',
            lastName: 'Nagy',
            fullName: 'Anna Nagy',
          },
          replies: [],
        },
      ],
    };
    
    setPost(mockPost);
    setIsLoading(false);
  }, [id]);

  const handleReply = async () => {
    if (!replyContent.trim()) return;

    // In real app: submit reply to API
    console.log('Reply submitted:', replyContent);
    setReplyContent('');
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('hu-HU', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!post) {
    return (
      <Container>
        <Typography>A poszt nem található</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 4 }}>
        <IconButton onClick={() => navigate('/forum')}>
          <ArrowBackIcon />
        </IconButton>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>
          {post.title}
          {post.isPinned && (
            <PushPinIcon 
              fontSize="small" 
              sx={{ ml: 1, verticalAlign: 'middle', color: 'warning.main' }} 
            />
          )}
        </Typography>
      </Box>

      {/* Main Post */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
            <Avatar src={post.author.profileImageUrl}>
              {post.author.firstName.charAt(0)}
            </Avatar>
            <Box sx={{ flexGrow: 1 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                <Typography variant="h6">
                  {post.author.fullName}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {formatDate(post.createdAt)}
                </Typography>
              </Box>
              <Typography variant="body1" paragraph>
                {post.content}
              </Typography>
              {user?.id === post.author.id && (
                <Box>
                  <IconButton size="small">
                    <EditIcon fontSize="small" />
                  </IconButton>
                  <IconButton size="small">
                    <DeleteIcon fontSize="small" />
                  </IconButton>
                </Box>
              )}
            </Box>
          </Box>
        </CardContent>
      </Card>

      {/* Replies */}
      <Typography variant="h6" gutterBottom>
        Válaszok ({post.replies.length})
      </Typography>
      
      {post.replies.map((reply) => (
        <Card key={reply.id} sx={{ mb: 2, ml: 4 }}>
          <CardContent>
            <Box sx={{ display: 'flex', gap: 2 }}>
              <Avatar src={reply.author.profileImageUrl} sx={{ width: 32, height: 32 }}>
                {reply.author.firstName.charAt(0)}
              </Avatar>
              <Box sx={{ flexGrow: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                  <Typography variant="subtitle2">
                    {reply.author.fullName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {formatDate(reply.createdAt)}
                  </Typography>
                </Box>
                <Typography variant="body2">
                  {reply.content}
                </Typography>
              </Box>
            </Box>
          </CardContent>
        </Card>
      ))}

      {/* Reply Form */}
      <Card sx={{ mt: 4 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Válasz írása
          </Typography>
          <TextField
            fullWidth
            multiline
            rows={4}
            placeholder="Írd ide a válaszod..."
            value={replyContent}
            onChange={(e) => setReplyContent(e.target.value)}
            sx={{ mb: 2 }}
          />
          <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
            <Button
              variant="contained"
              startIcon={<ReplyIcon />}
              onClick={handleReply}
              disabled={!replyContent.trim()}
            >
              Válasz küldése
            </Button>
          </Box>
        </CardContent>
      </Card>
    </Container>
  );
};

export default ForumPost;