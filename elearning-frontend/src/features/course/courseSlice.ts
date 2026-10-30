import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { courseService } from '../../services/courseService';
import { Course, CourseDetails } from '../../types/course';
import toast from 'react-hot-toast';

interface CourseState {
  courses: Course[];
  currentCourse: CourseDetails | null;
  myCourses: Course[];
  teachingCourses: Course[];
  isLoading: boolean;
  error: string | null;
}

const initialState: CourseState = {
  courses: [],
  currentCourse: null,
  myCourses: [],
  teachingCourses: [],
  isLoading: false,
  error: null,
};

// Async thunks
export const fetchCourses = createAsyncThunk(
  'courses/fetchCourses',
  async (_, { rejectWithValue }) => {
    try {
      return await courseService.getAllCourses();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Kurzusok betöltése sikertelen');
      return rejectWithValue(error.response?.data?.message);
    }
  }
);

export const fetchCourseDetails = createAsyncThunk(
  'courses/fetchCourseDetails',
  async (id: number, { rejectWithValue }) => {
    try {
      return await courseService.getCourseDetails(id);
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Kurzus betöltése sikertelen');
      return rejectWithValue(error.response?.data?.message);
    }
  }
);

export const enrollInCourse = createAsyncThunk(
  'courses/enroll',
  async (id: number, { rejectWithValue }) => {
    try {
      await courseService.enrollInCourse(id);
      toast.success('Sikeres feliratkozás a kurzusra!');
      return id;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Feliratkozás sikertelen');
      return rejectWithValue(error.response?.data?.message);
    }
  }
);

export const fetchMyCourses = createAsyncThunk(
  'courses/fetchMyCourses',
  async (_, { rejectWithValue }) => {
    try {
      return await courseService.getMyCourses();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Kurzusaim betöltése sikertelen');
      return rejectWithValue(error.response?.data?.message);
    }
  }
);

export const fetchTeachingCourses = createAsyncThunk(
  'courses/fetchTeachingCourses',
  async (_, { rejectWithValue }) => {
    try {
      return await courseService.getTeachingCourses();
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Tanított kurzusok betöltése sikertelen');
      return rejectWithValue(error.response?.data?.message);
    }
  }
);

// Remove rateCourse action for now
// We'll add it back when we implement it

// Slice
const courseSlice = createSlice({
  name: 'courses',
  initialState,
  reducers: {
    clearCurrentCourse: (state) => {
      state.currentCourse = null;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch courses
    builder.addCase(fetchCourses.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchCourses.fulfilled, (state, action) => {
      state.isLoading = false;
      state.courses = action.payload;
    });
    builder.addCase(fetchCourses.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Fetch course details
    builder.addCase(fetchCourseDetails.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(fetchCourseDetails.fulfilled, (state, action) => {
      state.isLoading = false;
      state.currentCourse = action.payload;
    });
    builder.addCase(fetchCourseDetails.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    // Enroll in course
    builder.addCase(enrollInCourse.fulfilled, (state, action) => {
      const courseId = action.payload;
      const course = state.courses.find(c => c.id === courseId);
      if (course) {
        course.isEnrolled = true;
      }
      if (state.currentCourse?.id === courseId) {
        state.currentCourse.isEnrolled = true;
      }
    });

    // Fetch my courses
    builder.addCase(fetchMyCourses.fulfilled, (state, action) => {
      state.myCourses = action.payload;
    });

    // Fetch teaching courses
    builder.addCase(fetchTeachingCourses.fulfilled, (state, action) => {
      state.teachingCourses = action.payload;
    });
  },
});

export const { clearCurrentCourse, clearError } = courseSlice.actions;
export default courseSlice.reducer;