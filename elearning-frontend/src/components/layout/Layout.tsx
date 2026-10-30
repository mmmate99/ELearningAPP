import React from 'react';
import { Outlet } from 'react-router-dom';
import { Box, Container } from '@mui/material';
import Navbar from './Navbar';
import Sidebar from './Sidebar';
import NotificationCenter from '../common/NotificationCenter';

const Layout: React.FC = () => {
  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <Navbar />
      <Sidebar />
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          marginTop: '64px',
          marginLeft: '240px',
          minHeight: 'calc(100vh - 64px)',
        }}
      >
        <Container maxWidth="xl">
          <Outlet />
        </Container>
      </Box>
      <NotificationCenter />
    </Box>
  );
};

export default Layout;