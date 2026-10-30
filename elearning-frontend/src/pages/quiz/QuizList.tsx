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
  CircularProgress,
} from '@mui/material';
import {
  Search as SearchIcon,
  Quiz as QuizIcon,
  AccessTime as AccessTimeIcon,
  School as SchoolIcon,
  TrendingUp as TrendingUpIcon,
} from '@mui/icons-material';
//import { quizService } from '../../services/quizService';
import { Quiz } from '../../types/quiz';

const QuizList: React.FC = () => {
  const navigate = useNavigate();
  //const { user } = useAppSelector((state) => state.auth);
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadQuizzes();
  }, []);

  const loadQuizzes = async () => {
    try {
      setIsLoading(true);
      // This would come from backend - for now mock data
      const mockQuizzes: Quiz[] = [
        {
          id: 1,
          title: 'JavaScript Alapok',
          description: 'Teszteld JavaScript ismereteidet',
          passPercentage: 70,
          timeLimit: 30,
          isPublished: true,
          maxAttempts: 3,
          courseId: 1,
          questions: [],
        },
        {
          id: 2,
          title: 'React Hooks',
          description: 'React Hooks ismeretek ellenőrzése',
          passPercentage: 80,
          timeLimit: 45,
          isPublished: true,
          maxAttempts: 2,
          courseId: 1,
          questions: [],
        },
        {
          id: 3,
          title: 'TypeScript Haladó',
          description: 'Haladó TypeScript kérdések',
          passPercentage: 85,
          timeLimit: 60,
          isPublished: true,
          maxAttempts: 1,
          courseId: 2,
          questions: [],
        },
      ];
      setQuizzes(mockQuizzes);
    } catch (error) {
      console.error('Error loading quizzes:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredQuizzes = quizzes.filter(quiz =>
    quiz.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    quiz.description?.toLowerCase().includes(searchTerm.toLowerCase())
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
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom>
          Kvízek
        </Typography>
        <Typography variant="body1" color="text.secondary" gutterBottom>
          Gyakorold tudásod kvízek kitöltésével
        </Typography>
      </Box>

      <Box sx={{ mb: 4 }}>
        <TextField
          fullWidth
          variant="outlined"
          placeholder="Keresés kvízek között..."
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
        {filteredQuizzes.map((quiz) => (
          <Grid item xs={12} sm={6} md={4} key={quiz.id}>
            <Card
              sx={{
                height: '100%',
                display: 'flex',
                flexDirection: 'column',
                transition: 'transform 0.2s',
                '&:hover': {
                  transform: 'translateY(-4px)',
                },
              }}
            >
              <CardContent sx={{ flexGrow: 1 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                  <QuizIcon color="primary" />
                  <Typography variant="h6">
                    {quiz.title}
                  </Typography>
                </Box>
                <Typography variant="body2" color="text.secondary" paragraph>
                  {quiz.description}
                </Typography>
                <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 2 }}>
                  <Chip
                    icon={<AccessTimeIcon />}
                    label={`${quiz.timeLimit} perc`}
                    size="small"
                    variant="outlined"
                  />
                  <Chip
                    icon={<TrendingUpIcon />}
                    label={`${quiz.passPercentage}% átmeneti`}
                    size="small"
                    variant="outlined"
                  />
                  <Chip
                    icon={<SchoolIcon />}
                    label={`${quiz.maxAttempts} próba`}
                    size="small"
                    variant="outlined"
                  />
                </Box>
              </CardContent>
              <CardActions>
                <Button
                  variant="contained"
                  fullWidth
                  onClick={() => navigate(`/quizzes/${quiz.id}`)}
                >
                  Kvíz indítása
                </Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>

      {filteredQuizzes.length === 0 && (
        <Box textAlign="center" py={8}>
          <QuizIcon sx={{ fontSize: 80, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" gutterBottom>
            Nincs találat
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Próbálj meg másik keresési kifejezést
          </Typography>
        </Box>
      )}
    </Box>
  );
};

export default QuizList;