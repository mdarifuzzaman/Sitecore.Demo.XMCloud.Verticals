import '@testing-library/jest-dom'
import { render } from '@testing-library/react'
import { Default } from '../src/components/TestComponent'
 
describe('TestComponent should not be null', () => {
  it('renders a TestComponent', () => {
    const renderedData = render(<Default/>);
    expect(renderedData).not.toBeNull();
  })
})