import React from 'react';
import { Box, Typography, LinearProgress, Chip } from '@mui/material';
import { CheckCircle as CheckCircleIcon } from '@mui/icons-material';

interface CourseProgressProps {
  totalLessons: number;
  completedLessons: number;
  totalDuration: number;
  timeSpent: number;
}

const CourseProgress: React.FC<CourseProgressProps> = ({
  totalLessons,
  completedLessons,
  totalDuration,
  timeSpent,
}) => {
  const progressPercent = totalLessons > 0 ? (completedLessons / totalLessons) * 100 : 0;

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
        <Typography variant="body2">
          Teljesített leckék
        </Typography>
        <Chip
          icon={<CheckCircleIcon />}
          label={`${completedLessons}/${totalLessons}`}
          size="small"
          color={completedLessons === totalLessons ? 'success' : 'primary'}
        />
      </Box>
      <LinearProgress 
        variant="determinate" 
        value={progressPercent} 
        sx={{ height: 8, borderRadius: 4, mb: 2 }}
      />
      <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
        <Typography variant="caption" color="text.secondary">
          Becsült idő: {Math.floor(totalDuration / 60)}h
        </Typography>
        <Typography variant="caption" color="text.secondary">
          Eltöltött idő: {Math.floor(timeSpent / 3600)}h {Math.floor((timeSpent % 3600) / 60)}m
        </Typography>
      </Box>
    </Box>
  );
};

export default CourseProgress;