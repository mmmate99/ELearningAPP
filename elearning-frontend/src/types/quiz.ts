export interface Quiz {
  id: number;
  title: string;
  description?: string;
  passPercentage: number;
  timeLimit: number;
  isPublished: boolean;
  maxAttempts: number;
  lessonId?: number;
  courseId?: number;
  questions: Question[];
}

export interface Question {
  id: number;
  questionText: string;
  type: QuestionType;
  points: number;
  order: number;
  answerOptions: AnswerOption[];
  correctAnswer?: string;
  matchingPairsJson?: string;
  sortingItemsJson?: string;
}

export interface AnswerOption {
  id: number;
  optionText: string;
  isCorrect: boolean;
  order: number;
}

export interface QuizAttempt {
  quizId: number;
  answers: QuestionAttempt[];
}

export interface QuestionAttempt {
  questionId: number;
  selectedOptionIds: number[];
  textAnswer?: string;
  matchingPairs?: Record<string, string>;
  sortedItems?: string[];
}

export interface QuizResult {
  quizId: number;
  score: number;
  totalPoints: number;
  percentage: number;
  isPassed: boolean;
  attemptNumber: number;
  questionResults: QuestionResult[];
}

export interface QuestionResult {
  questionId: number;
  isCorrect: boolean;
  pointsEarned: number;
  totalPoints: number;
  correctAnswer?: string;
  userAnswer?: string;
}

export enum QuestionType {
  MultipleChoice = 'MultipleChoice',
  TrueFalse = 'TrueFalse',
  ShortAnswer = 'ShortAnswer',
  Matching = 'Matching',
  Sorting = 'Sorting'
}