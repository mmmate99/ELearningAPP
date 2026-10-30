import React, { useState } from 'react';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Switch,
  FormControlLabel,
  Alert,
  CircularProgress,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
} from '@mui/material';
import {
  Save as SaveIcon,
  Security as SecurityIcon,
  Notifications as NotificationsIcon,
  //Language as LanguageIcon,
  Palette as PaletteIcon,
} from '@mui/icons-material';

const Settings: React.FC = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [saveSuccess, setSaveSuccess] = useState(false);
  const [settings, setSettings] = useState({
    notifications: {
      emailNotifications: true,
      pushNotifications: true,
      courseUpdates: true,
      forumReplies: true,
    },
    privacy: {
      showProfile: true,
      showCourses: true,
      showProgress: false,
    },
    preferences: {
      theme: 'light',
      language: 'hu',
      timezone: 'Europe/Budapest',
    },
  });

  const handleToggle = (category: keyof typeof settings, key: string) => {
    setSettings(prev => ({
      ...prev,
      [category]: {
        ...prev[category],
        [key]: !(prev[category] as any)[key],
      },
    }));
  };

  const handleSelectChange = (category: keyof typeof settings, key: string, value: string) => {
    setSettings(prev => ({
      ...prev,
      [category]: {
        ...prev[category],
        [key]: value,
      },
    }));
  };

  const handleSave = async () => {
    try {
      setIsLoading(true);
      // Save settings to backend
      await new Promise(resolve => setTimeout(resolve, 1000)); // Simulate API call
      setSaveSuccess(true);
      setTimeout(() => setSaveSuccess(false), 3000);
    } catch (error) {
      console.error('Error saving settings:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" gutterBottom>
        Beállítások
      </Typography>

      {saveSuccess && (
        <Alert severity="success" sx={{ mb: 3 }}>
          Beállítások sikeresen mentve!
        </Alert>
      )}

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
        {/* Notification Settings */}
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 3 }}>
            <NotificationsIcon color="primary" />
            <Typography variant="h6">Értesítések</Typography>
          </Box>
          
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <FormControlLabel
              control={
                <Switch
                  checked={settings.notifications.emailNotifications}
                  onChange={() => handleToggle('notifications', 'emailNotifications')}
                />
              }
              label="Email értesítések"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.notifications.pushNotifications}
                  onChange={() => handleToggle('notifications', 'pushNotifications')}
                />
              }
              label="Push értesítések"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.notifications.courseUpdates}
                  onChange={() => handleToggle('notifications', 'courseUpdates')}
                />
              }
              label="Kurzus frissítések"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.notifications.forumReplies}
                  onChange={() => handleToggle('notifications', 'forumReplies')}
                />
              }
              label="Fórum válaszok"
            />
          </Box>
        </Paper>

        {/* Privacy Settings */}
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 3 }}>
            <SecurityIcon color="primary" />
            <Typography variant="h6">Adatvédelem</Typography>
          </Box>
          
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <FormControlLabel
              control={
                <Switch
                  checked={settings.privacy.showProfile}
                  onChange={() => handleToggle('privacy', 'showProfile')}
                />
              }
              label="Profil megjelenítése másoknak"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.privacy.showCourses}
                  onChange={() => handleToggle('privacy', 'showCourses')}
                />
              }
              label="Kurzusaim megjelenítése"
            />
            <FormControlLabel
              control={
                <Switch
                  checked={settings.privacy.showProgress}
                  onChange={() => handleToggle('privacy', 'showProgress')}
                />
              }
              label="Haladásom megjelenítése"
            />
          </Box>
        </Paper>

        {/* Preferences */}
        <Paper sx={{ p: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 3 }}>
            <PaletteIcon color="primary" />
            <Typography variant="h6">Preferenciák</Typography>
          </Box>
          
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
            <FormControl fullWidth>
              <InputLabel>Téma</InputLabel>
              <Select
                value={settings.preferences.theme}
                label="Téma"
                onChange={(e) => handleSelectChange('preferences', 'theme', e.target.value)}
              >
                <MenuItem value="light">Világos</MenuItem>
                <MenuItem value="dark">Sötét</MenuItem>
                <MenuItem value="auto">Automatikus</MenuItem>
              </Select>
            </FormControl>

            <FormControl fullWidth>
              <InputLabel>Nyelv</InputLabel>
              <Select
                value={settings.preferences.language}
                label="Nyelv"
                onChange={(e) => handleSelectChange('preferences', 'language', e.target.value)}
              >
                <MenuItem value="hu">Magyar</MenuItem>
                <MenuItem value="en">English</MenuItem>
                <MenuItem value="de">Deutsch</MenuItem>
              </Select>
            </FormControl>

            <FormControl fullWidth>
              <InputLabel>Időzóna</InputLabel>
              <Select
                value={settings.preferences.timezone}
                label="Időzóna"
                onChange={(e) => handleSelectChange('preferences', 'timezone', e.target.value)}
              >
                <MenuItem value="Europe/Budapest">Budapest (GMT+1)</MenuItem>
                <MenuItem value="Europe/London">London (GMT+0)</MenuItem>
                <MenuItem value="America/New_York">New York (GMT-5)</MenuItem>
              </Select>
            </FormControl>
          </Box>
        </Paper>

        {/* Save Button */}
        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          <Button
            variant="contained"
            size="large"
            startIcon={isLoading ? <CircularProgress size={20} /> : <SaveIcon />}
            onClick={handleSave}
            disabled={isLoading}
          >
            {isLoading ? 'Mentés...' : 'Beállítások mentése'}
          </Button>
        </Box>
      </Box>
    </Container>
  );
};

export default Settings;