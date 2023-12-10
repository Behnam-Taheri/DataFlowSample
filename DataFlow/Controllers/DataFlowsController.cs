using System.Threading.Tasks.Dataflow;
using DataFlow.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DataFlow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataFlowsController : ControllerBase
    {
        private readonly BufferBlock<BufferBlockRequest> _bufferBlock;
        public DataFlowsController(BufferBlock<BufferBlockRequest> bufferBlock)
        {
            _bufferBlock = bufferBlock;
        }

        [HttpPost("ActionBlockDemo")]
        public void ActionBlockDemo()
        {
            var actionBlock = new ActionBlock<int>(async request =>
            {
                await ComplicatedComputation(request);
                Console.WriteLine(request);
            }
            , new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
                EnsureOrdered = false
            });

            actionBlock.Post(1);
            actionBlock.Post(2);
            actionBlock.Post(3);
            actionBlock.Post(4);
        }

        /// <summary>
        /// Config For FIFO
        /// </summary>
        [HttpPost("BufferBlockSimpleDemo")]
        public void BufferBlockSimpleDemo()
        {
            BufferBlock<int> bufferBlock = new BufferBlock<int>();

            var actionBlock = new ActionBlock<int>(async request =>
            {
                await ComplicatedComputation(request);
                Console.WriteLine(request);
            },
            new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );


            bufferBlock.LinkTo(actionBlock);

            bufferBlock.Post(1);
            bufferBlock.Post(2);
            bufferBlock.Post(3);
            bufferBlock.Post(4);
        }

        [HttpPost("BufferBlockDemo")]
        public void BufferBlockDemo(BufferBlockRequest request)
        {
            var actionBlock = new ActionBlock<BufferBlockRequest>(async x =>
            {
                await ComplicatedComputation(x.Id);
                Console.WriteLine(x);
            },
            new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );

            _bufferBlock.LinkTo(actionBlock);
            _bufferBlock.Post(request);
        }


        private async Task ComplicatedComputation(int request)
        {
            if (request == 1)
            {
                Thread.Sleep(5000);
            }

            if (request == 2)
            {
                Thread.Sleep(1000);
            }

            await Task.CompletedTask;
        }

        private async Task ComplicatedComputation(Guid id)
        {

            await Task.CompletedTask;
        }
    }
}