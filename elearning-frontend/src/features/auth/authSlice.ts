import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { authService } from '../../services/authService';
import { AuthState, LoginRequest, RegisterRequest } from '../../types/auth';
import toast from 'react-hot-toast';

const initialState: AuthState = {
  user: authService.getStoredUser(),
  token: authService.getStoredToken(),
  isLoading: false,
  error: null,
};

// Async thunks - remove unused User import
export const login = createAsyncThunk(
  'auth/login',
  async (credentials: LoginRequest, { rejectWithValue }) => {
    try {
      const response = await authService.login(credentials);
      authService.saveAuthData(response);
      toast.success('Sikeres bejelentkezés!');
      return response;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Bejelentkezés sikertelen');
      return rejectWithValue(error.response?.data?.message || 'Bejelentkezés sikertelen');
    }
  }
);

export const register = createAsyncThunk(
  'auth/register',
  async (userData: RegisterRequest, { rejectWithValue }) => {
    try {
      const response = await authService.register(userData);
      authService.saveAuthData(response);
      toast.success('Sikeres regisztráció!');
      return response;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Regisztráció sikertelen');
      return rejectWithValue(error.response?.data?.message || 'Regisztráció sikertelen');
    }
  }
);

export const logout = createAsyncThunk('auth/logout', async () => {
  authService.logout();
  toast.success('Sikeres kijelentkezés!');
});

export const getProfile = createAsyncThunk(
  'auth/getProfile',
  async (_, { rejectWithValue }) => {
    try {
      return await authService.getProfile();
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Profil betöltése sikertelen');
    }
  }
);

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setUser: (state, action) => {
      state.user = action.payload;
    },
  },
  extraReducers: (builder) => {
    // ... existing extraReducers
    builder.addCase(login.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(login.fulfilled, (state, action) => {
      state.isLoading = false;
      state.user = action.payload.user;
      state.token = action.payload.token;
    });
    builder.addCase(login.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    builder.addCase(register.pending, (state) => {
      state.isLoading = true;
      state.error = null;
    });
    builder.addCase(register.fulfilled, (state, action) => {
      state.isLoading = false;
      state.user = action.payload.user;
      state.token = action.payload.token;
    });
    builder.addCase(register.rejected, (state, action) => {
      state.isLoading = false;
      state.error = action.payload as string;
    });

    builder.addCase(logout.fulfilled, (state) => {
      state.user = null;
      state.token = null;
    });

    builder.addCase(getProfile.pending, (state) => {
      state.isLoading = true;
    });
    builder.addCase(getProfile.fulfilled, (state, action) => {
      state.isLoading = false;
      state.user = action.payload;
    });
    builder.addCase(getProfile.rejected, (state) => {
      state.isLoading = false;
      state.user = null;
      state.token = null;
    });
  },
});

export const { clearError, setUser } = authSlice.actions;
export default authSlice.reducer;