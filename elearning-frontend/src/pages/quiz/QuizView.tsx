import React, { useState, useEffect } from 'react';
import { useParams, /*useNavigate*/ } from 'react-router-dom';
//import { useAppSelector } from '../../store/hooks';
import {
  Container,
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  TextField,
  LinearProgress,
  Alert,
} from '@mui/material';
import { Quiz, /*Question,*/ QuestionType } from '../../types/quiz';

const QuizView: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  //const navigate = useNavigate();
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [currentQuestion, setCurrentQuestion] = useState(0);
  const [answers, setAnswers] = useState<Record<number, any>>({});
  const [timeLeft, setTimeLeft] = useState(1800); // 30 minutes in seconds
  const [isSubmitted, setIsSubmitted] = useState(false);

  // Mock quiz data
  useEffect(() => {
    const mockQuiz: Quiz = {
      id: 1,
      title: 'JavaScript Alapok',
      description: 'Teszteld JavaScript ismereteidet',
      passPercentage: 70,
      timeLimit: 30,
      isPublished: true,
      maxAttempts: 3,
      courseId: 1,
      questions: [
        {
          id: 1,
          questionText: 'Mi a JavaScript?',
          type: QuestionType.MultipleChoice,
          points: 10,
          order: 1,
          answerOptions: [
            { id: 1, optionText: 'Programozási nyelv', isCorrect: true, order: 1 },
            { id: 2, optionText: 'Kávéfajta', isCorrect: false, order: 2 },
            { id: 3, optionText: 'Operációs rendszer', isCorrect: false, order: 3 },
          ],
        },
        {
          id: 2,
          questionText: 'Írj egy függvényt, ami összead két számot',
          type: QuestionType.ShortAnswer,
          points: 20,
          order: 2,
          correctAnswer: 'function add(a, b) { return a + b; }',
          answerOptions: [],
        },
      ],
    };
    setQuiz(mockQuiz);
    setTimeLeft(mockQuiz.timeLimit * 60);
  }, [id]);

  // Timer
  useEffect(() => {
    if (timeLeft <= 0 || isSubmitted) return;

    const timer = setInterval(() => {
      setTimeLeft((prev) => prev - 1);
    }, 1000);

    return () => clearInterval(timer);
  }, [timeLeft, isSubmitted]);

  const handleAnswerChange = (questionId: number, answer: any) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: answer,
    }));
  };

  const handleNext = () => {
    if (quiz && currentQuestion < quiz.questions.length - 1) {
      setCurrentQuestion(currentQuestion + 1);
    }
  };

  const handlePrev = () => {
    if (currentQuestion > 0) {
      setCurrentQuestion(currentQuestion - 1);
    }
  };

  const handleSubmit = () => {
    setIsSubmitted(true);
    console.log('Quiz submitted:', answers);
    // In real app: submit to API
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs < 10 ? '0' : ''}${secs}`;
  };

  if (!quiz) {
    return (
      <Container>
        <Typography>Quiz betöltése...</Typography>
      </Container>
    );
  }

  const currentQ = quiz.questions[currentQuestion];
  const progress = ((currentQuestion + 1) / quiz.questions.length) * 100;

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Typography variant="h4">{quiz.title}</Typography>
            <Typography variant="h6" color={timeLeft < 300 ? 'error' : 'primary'}>
              {formatTime(timeLeft)}
            </Typography>
          </Box>
          <Typography variant="body1" color="text.secondary" paragraph>
            {quiz.description}
          </Typography>
          <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', mb: 2 }}>
            <Typography variant="body2">
              {currentQuestion + 1} / {quiz.questions.length} kérdés
            </Typography>
            <Typography variant="body2">
              {quiz.passPercentage}% átmeneti pontszám
            </Typography>
          </Box>
          <LinearProgress variant="determinate" value={progress} />
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            {currentQ.questionText} ({currentQ.points} pont)
          </Typography>

          {currentQ.type === QuestionType.MultipleChoice && (
            <FormControl component="fieldset">
              <RadioGroup
                value={answers[currentQ.id] || ''}
                onChange={(e) => handleAnswerChange(currentQ.id, e.target.value)}
              >
                {currentQ.answerOptions.map((option) => (
                  <FormControlLabel
                    key={option.id}
                    value={option.id.toString()}
                    control={<Radio />}
                    label={option.optionText}
                  />
                ))}
              </RadioGroup>
            </FormControl>
          )}

          {currentQ.type === QuestionType.TrueFalse && (
            <FormControl component="fieldset">
              <RadioGroup
                value={answers[currentQ.id] || ''}
                onChange={(e) => handleAnswerChange(currentQ.id, e.target.value)}
              >
                <FormControlLabel value="true" control={<Radio />} label="Igaz" />
                <FormControlLabel value="false" control={<Radio />} label="Hamis" />
              </RadioGroup>
            </FormControl>
          )}

          {currentQ.type === QuestionType.ShortAnswer && (
            <TextField
              fullWidth
              multiline
              rows={4}
              placeholder="Írd ide a válaszod..."
              value={answers[currentQ.id] || ''}
              onChange={(e) => handleAnswerChange(currentQ.id, e.target.value)}
            />
          )}

          <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 4 }}>
            <Button
              onClick={handlePrev}
              disabled={currentQuestion === 0}
            >
              Előző kérdés
            </Button>
            
            {currentQuestion === quiz.questions.length - 1 ? (
              <Button
                variant="contained"
                onClick={handleSubmit}
                disabled={isSubmitted}
              >
                Kvíz befejezése
              </Button>
            ) : (
              <Button
                variant="contained"
                onClick={handleNext}
              >
                Következő kérdés
              </Button>
            )}
          </Box>
        </CardContent>
      </Card>

      {isSubmitted && (
        <Alert severity="success" sx={{ mt: 3 }}>
          A kvíz elküldésre került! Az eredmény hamarosan megjelenik.
        </Alert>
      )}
    </Container>
  );
};

export default QuizView;