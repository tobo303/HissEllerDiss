import './App.css';
import { FaRegThumbsDown } from "react-icons/fa";
import { FaRegThumbsUp } from "react-icons/fa";

import React, { useState, useEffect } from 'react';
import axios from 'axios';

function App() {
  const [posts, setPosts] = useState([]);
  
  useEffect(() => {
    const apiUrl = process.env.REACT_APP_API_BASE_URL;
    axios.get('http://${apiUrl}:5208/api/hissellerdiss')
      .then(response => {
        setPosts(response.data);
      })
      .catch(error => {
        console.error(error);
      });
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        Hiss eller Diss
      </header>
      
      <ListOfLikes list={posts} />
      </div>
  );
}

function ListOfLikes({list}) {
  return (
    <table align='center'>
      <tbody>
      {list.map(entry => (
        <LikeItem entry={entry} />
      ))}
    </tbody>
  </table>
  );
}

function LikeItem({ entry }) {
  return (
        <tr>
          <td width={300}>
            {entry.name}
          </td>
          <td>
            <FaRegThumbsUp />
          </td>
          <td>
            <FaRegThumbsDown /> 
          </td>
          <td>
            Votes: {entry.likes}
          </td>
        </tr>
  );
}


export default App;
