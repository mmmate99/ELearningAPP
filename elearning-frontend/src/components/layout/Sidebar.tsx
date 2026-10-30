import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../../store/hooks';
import {
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Box,
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  School as SchoolIcon,
  Quiz as QuizIcon,
  Forum as ForumIcon,
  Bookmark as BookmarkIcon,
  Settings as SettingsIcon,
  VideoLibrary as VideoLibraryIcon,
  Assignment as AssignmentIcon,
} from '@mui/icons-material';

const drawerWidth = 240;

const Sidebar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAppSelector((state) => state.auth);

  const menuItems = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
    { text: 'Kurzusok', icon: <SchoolIcon />, path: '/courses' },
    { text: 'Kurzusaim', icon: <BookmarkIcon />, path: '/my-courses' },
    { text: 'Kvízek', icon: <QuizIcon />, path: '/quizzes' },
    { text: 'Fórum', icon: <ForumIcon />, path: '/forum' },
  ];

  if (user?.role === 'Teacher' || user?.role === 'Admin') {
    menuItems.push(
      { text: 'Tanított kurzusok', icon: <VideoLibraryIcon />, path: '/teaching' },
      { text: 'Kvíz létrehozás', icon: <AssignmentIcon />, path: '/create-quiz' }
    );
  }

  menuItems.push({ text: 'Beállítások', icon: <SettingsIcon />, path: '/settings' });

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: {
          width: drawerWidth,
          boxSizing: 'border-box',
          marginTop: '64px',
        },
      }}
    >
      <Box sx={{ overflow: 'auto', mt: 2 }}>
        <List>
          {menuItems.map((item) => (
            <ListItem
              key={item.text}
              onClick={() => navigate(item.path)}
              selected={location.pathname === item.path}
              sx={{
                cursor: 'pointer',
                '&.Mui-selected': {
                  backgroundColor: 'primary.main',
                  color: 'white',
                  '& .MuiListItemIcon-root': {
                    color: 'white',
                  },
                },
                '&.Mui-selected:hover': {
                  backgroundColor: 'primary.dark',
                },
              }}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItem>
          ))}
        </List>
        <Divider />
      </Box>
    </Drawer>
  );
};

export default Sidebar;