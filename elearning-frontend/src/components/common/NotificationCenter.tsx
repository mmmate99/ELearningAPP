import React, { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { markNotificationAsRead, removeNotification, clearNotifications } from '../../features/ui/uiSlice';
import {
  Box,
  IconButton,
  Badge,
  Popover,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Typography,
  Button,
  Divider,
} from '@mui/material';
import {
  Notifications as NotificationsIcon,
  Circle as CircleIcon,
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
  Warning as WarningIcon,
  Info as InfoIcon,
  ClearAll as ClearAllIcon,
} from '@mui/icons-material';
import { formatDistanceToNow } from 'date-fns';
import { hu } from 'date-fns/locale';

const NotificationCenter: React.FC = () => {
  const dispatch = useAppDispatch();
  const { notifications } = useAppSelector((state) => state.ui);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const getNotificationIcon = (type: string) => {
    switch (type) {
      case 'success':
        return <CheckCircleIcon color="success" />;
      case 'error':
        return <ErrorIcon color="error" />;
      case 'warning':
        return <WarningIcon color="warning" />;
      default:
        return <InfoIcon color="info" />;
    }
  };

  const unreadCount = notifications.filter(n => !n.read).length;

  return (
    <>
      <IconButton color="inherit" onClick={handleClick}>
        <Badge badgeContent={unreadCount} color="error">
          <NotificationsIcon />
        </Badge>
      </IconButton>

      <Popover
        open={Boolean(anchorEl)}
        anchorEl={anchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        sx={{ mt: 1 }}
      >
        <Box sx={{ width: 360, maxHeight: 400, overflow: 'auto' }}>
          <Box sx={{ p: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography variant="h6">Értesítések</Typography>
            {notifications.length > 0 && (
              <Button
                size="small"
                startIcon={<ClearAllIcon />}
                onClick={() => dispatch(clearNotifications())}
              >
                Összes törlése
              </Button>
            )}
          </Box>
          <Divider />
          <List>
            {notifications.length === 0 ? (
              <ListItem>
                <ListItemText primary="Nincsenek értesítések" />
              </ListItem>
            ) : (
              notifications.map((notification) => (
                <ListItem
                  key={notification.id}
                  sx={{
                    bgcolor: notification.read ? 'transparent' : 'action.hover',
                    borderLeft: notification.read ? 'none' : '4px solid',
                    borderColor: `${notification.type}.main`,
                  }}
                  secondaryAction={
                    <IconButton
                      edge="end"
                      size="small"
                      onClick={() => dispatch(removeNotification(notification.id))}
                    >
                      <CircleIcon fontSize="small" />
                    </IconButton>
                  }
                  onClick={() => dispatch(markNotificationAsRead(notification.id))}
                >
                  <ListItemIcon>
                    {getNotificationIcon(notification.type)}
                  </ListItemIcon>
                  <ListItemText
                    primary={notification.title}
                    secondary={
                      <>
                        <Typography component="span" variant="body2">
                          {notification.message}
                        </Typography>
                        <br />
                        <Typography variant="caption" color="text.secondary">
                          {formatDistanceToNow(notification.timestamp, {
                            addSuffix: true,
                            locale: hu,
                          })}
                        </Typography>
                      </>
                    }
                  />
                </ListItem>
              ))
            )}
          </List>
        </Box>
      </Popover>
    </>
  );
};

export default NotificationCenter;