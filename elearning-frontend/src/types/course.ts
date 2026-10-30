export interface Course {
  id: number;
  title: string;
  description: string;
  thumbnailUrl?: string;
  price: number;
  isFree: boolean;
  difficulty: DifficultyLevel;
  teacherName: string;
  teacherId: string;
  studentCount: number;
  averageRating?: number;
  isPublished: boolean;
  createdAt: string;
  isEnrolled: boolean;
  userRating?: number;
}

export interface CourseDetails extends Course {
  modules: Module[];
  quizzes: any[]; // Temporarily use any
  forumPosts: any[]; // Temporarily use any
}

export interface Module {
  id: number;
  title: string;
  description?: string;
  order: number;
  courseId: number;
  lessons: Lesson[];
}

export interface Lesson {
  id: number;
  title: string;
  content?: string;
  order: number;
  type: LessonType;
  videoUrl?: string;
  attachmentUrl?: string;
  estimatedDuration: number;
  moduleId: number;
  isCompleted: boolean;
  timeSpent?: number;
  currentTime?: number;
  previousLessonId?: number;
  nextLessonId?: number;
  moduleTitle?: string;
}

export interface CreateCourseRequest {
  title: string;
  description: string;
  thumbnailUrl?: string;
  price: number;
  isFree: boolean;
  difficulty: DifficultyLevel;
}

export enum DifficultyLevel {
  Beginner = 'Beginner',
  Intermediate = 'Intermediate',
  Advanced = 'Advanced'
}

export enum LessonType {
  Text = 'Text',
  Video = 'Video',
  Document = 'Document',
  Interactive = 'Interactive'
}

// Remove problematic imports for now
// import { Quiz } from './quiz';
// import { ForumPost } from './forum';
// import { User } from './auth';