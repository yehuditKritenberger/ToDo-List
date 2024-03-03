import axios from 'axios';

// הגדרת כתובת ה־API כברירת המחדל
axios.defaults.baseURL = "http://localhost:5195";

axios.interceptors.response.use(
  response => response,
  error => {
    // בדיקה האם יש שגיאת תגובה מהשרת
    if (error.response) {
      // הדפסת השגיאה ללוג
      console.error('Response Error:', error.response.status, error.response.data);
    } else if (error.request) {
      // בקשה נשלחה אך לא התקבלה תגובה
      console.error('Request Error:', error.request);
    } else {
      // שגיאה בעת עיבוד הבקשה
      console.error('Error:', error.message);
    }
    // החזרת השגיאה למעבדת הבקשות
    return Promise.reject(error);
  }
);

const service = {
  getTasks: async () => {
    const result = await axios.get('/items');
    return result.data;
  },
  addTask: async (name) => {
    console.log(name)
    await axios.post('/items', { Name: name ,isComplete:false});
  },

  setCompleted: async (id, isComplete) => {
    await axios.put(`/items/${id}`);
  },

  deleteTask: async (id) => {
    await axios.delete(`/items/${id}`);
  }
};

export default service;
