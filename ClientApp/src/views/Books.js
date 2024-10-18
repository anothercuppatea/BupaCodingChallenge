import React, { useEffect, useState } from 'react'
import CategoryContainer from '../components/CategoryContainer'
import axios from 'axios';


const Books = () => {
    const [books, setBooks] = useState(
        {
            child: [],
            adult: []
        }
    )
    const [hardcoverBooks, setHardcoverBooks] = useState(
        {
            child: [],
            adult: []
        }
    )
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isHardcover, setIsHardcover] = useState(false)

    useEffect(() => {
        const fetchbooks = async () => {
            try {
                const response = await axios.get('/api/books', { timeout: 10000 })
                setBooks(response.data)
                const response2 = await axios.get('/api/books/hardcover', { timeout: 10000 })
                setBooks(response.data)
                setHardcoverBooks(response2.data)
                setLoading(false)
            } catch (err) {
                setError(err.message)
                setLoading(false)
            }
        }
        fetchbooks();
    }, [])

    if (loading) return <div> Loading, please wait. </div>
    if (error) return <div> Something went wrong : {error}</div>
    const categoryStyle = 'flex flex-col md:flex-row gap-4 h-72 justify-center lg:my-2 xs:my-36 xs:mb-40' 
    return <div className='border-blue-600 mx-auto p-4'>
        <div className=' text-center font-bold text-3xl bg-blue-600 text-white p-3 mb-4 '>Owners and books</div>
        {
            isHardcover ? (<div className={categoryStyle}>
                <CategoryContainer header="Hardcover books owned by Adults" items={hardcoverBooks.adult} ></CategoryContainer>
                <CategoryContainer header="Hardcover books owned by Children" items={hardcoverBooks.child} ></CategoryContainer>
            </div>) : (<div className={categoryStyle}>
                <CategoryContainer header="Books owned by Adults" items={books.adult} ></CategoryContainer>
                <CategoryContainer header="Books owned by Children" items={books.child} ></CategoryContainer>
            </div>)
        }
        <hr class="my-2 h-0.5 border-t-0 bg-slate-500" />
        <div className='flex gap-2 justify-end'>
            <button className=' underline text-blue-600 font-bold text-md' onClick={()=>setIsHardcover(true)}>Hardcover only</button> <button className=' p-2 rounded-lg px-3 bg-blue-600 text-white' onClick={()=>setIsHardcover(false)}>Get books</button>
        </div>
    </div>
}
export default Books;