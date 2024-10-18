import React from 'react'

const CategoryContainer = (props) => {
    console.log(props.items)
    return <div className='flex-1 lg:drop-shadow-none lg:border-none xs:mx-auto xs:border-2 xs:p-2 xs:rounded-md xs:drop-shadow-lg bg-white'>
        <div className=' font-bold text-3xl text-center bg-blue-600 text-white py-3 mb-4 xs:px-2'>{props.header}</div>
        {
            props.items.length > 0 ? (
                props.items.map(i => (
                    <div>
                        <p>{i}</p>
                    </div>
                )))
                : (
                    <div>
                        No items are available that matches the criteria.
                    </div>
                )
        }
    </div>
}

export default CategoryContainer;
